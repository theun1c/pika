using System.Runtime.InteropServices;
using Pika.Playback.Mpv.Interop;

namespace Pika.Playback.Mpv.Core;

// связывает Render (UI) с MPV
public class MpvRenderBridge : IDisposable
{
    // флаг из render.h - есть новый кадр для отрисовки 
    private const ulong MpvRenderUpdateFrame = 1UL << 0; 
    
    // указатель на mpv_render_context*
    private nint _renderContext;
    // флаг освобождения
    private bool _isDisposed;
    
    // делегат, который UI передает для запроса перерисовки 
    private readonly Action _requestRedraw;
    
    // сохраним resolver GL-функции 
    private Func<string, nint> _getProcAddress;
    
    // gl-handle на текущий экземпляр, чтобы отдать его в callback ctx
    private GCHandle _selfHandle;
    
    // храним делегаты в полях, чтобы GC не собрал их
    private MpvGetProcAddressDelegate? _getProcAddressCallback;
    private MpvUpdateCallbackDelegate? _updateCallback;
    
    // callback сигнатура для mpv_opengl_init_params.get_proc_address
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nint MpvGetProcAddressDelegate(nint ctx, [MarshalAs(UnmanagedType.LPUTF8Str)] string name);
    
    // callback сигнатура для update callback
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void MpvUpdateCallbackDelegate(nint ctx);

    // передаем действие, которое просит UI перерисовать кадр
    public MpvRenderBridge(Action requestRedraw)
    {
        _requestRedraw = requestRedraw ?? throw new ArgumentNullException(nameof(requestRedraw));
    }
    
    // флаг состояния
    public bool IsInitialized => _renderContext != nint.Zero;
    
    // инициализация render context должна идти на GL-поток
    public void InitializeFromOpenGl(nint mpvHandle, Func<string, nint> getProcAddress)
    {
        ThrowIfDisposed();
        if (mpvHandle == nint.Zero)
            throw new ArgumentException($"mpv handle is zero", nameof(mpvHandle));
        
        if (getProcAddress is null)
            throw new ArgumentNullException($"get_proc_address is null", nameof(getProcAddress));

        if (_renderContext != nint.Zero)
            throw new InvalidOperationException("render context is already initialized");
        
        _getProcAddress = getProcAddress;
        
        // готовим callback'и и фиксируем this через GCHandle
        _getProcAddressCallback = GetProcAddressTrampoline;
        _updateCallback = UpdateTrampoline;
        _selfHandle = GCHandle.Alloc(this, GCHandleType.Normal);
        
        // unmanaged-указатели, которые нужно освободить
        nint apiTypePtr = nint.Zero;
        nint glInitPtr = nint.Zero;
        nint paramsPtr = nint.Zero;

        try
        {
            // MPV_RENDER_PARAM_API_TYPE = "opengl"
            apiTypePtr = Marshal.StringToCoTaskMemUTF8("opengl");

            // MPV_RENDER_PARAM_OPENGL_INIT_PARAMS
            var glInit = new MpvNative.MpvOpenGlInitParams
            {
                // адрес callbacka, который mpv будет вызывать для резолва GL-функции
                GetProcAddress = Marshal.GetFunctionPointerForDelegate(_getProcAddressCallback),
                GetProcAddressCtx = GCHandle.ToIntPtr(_selfHandle),
            };
             
            glInitPtr = AllocStruct(glInit);

            // массив mpv_render_param[], обязательно завершить type=Invalid (0)
            var renderParams = new[]
            {
                new MpvNative.MpvRenderParam { Type = MpvNative.MpvRenderParamType.ApiType, Data = apiTypePtr },
                new MpvNative.MpvRenderParam { Type = MpvNative.MpvRenderParamType.OpenGlInitParams, Data = glInitPtr },
                new MpvNative.MpvRenderParam { Type = MpvNative.MpvRenderParamType.Invalid, Data = nint.Zero },
            };

            paramsPtr = AllocArray(renderParams);

            // создаем mpv_render_context*
            var code = MpvNative.mpv_render_context_create(out _renderContext, mpvHandle, paramsPtr);
            ThrowIfMpvError(code, "mpv_render_context_create");
            
            // подписываемся на уведомление о новых кадрах
            var updateFnPtr = Marshal.GetFunctionPointerForDelegate(_updateCallback);
            MpvNative.mpv_render_context_set_update_callback(_renderContext, updateFnPtr, GCHandle.ToIntPtr(_selfHandle));
        }
        finally
        {
            FreeIfNotZero(paramsPtr);
            FreeIfNotZero(glInitPtr);
            FreeIfNotZero(apiTypePtr);
        }
        
    }

    // вызываем перед Render(): true => есть новый кадр
    public bool Update()
    {
        ThrowIfDisposed();
        
        if (_renderContext == nint.Zero)
            return false;
        
        var flags = MpvNative.mpv_render_context_update(_renderContext);
        return (flags & MpvRenderUpdateFrame) != 0;
    }
    
    // фактический рендер в framebuffer, который дает avalonia 
    public void Render(int framebuffer, int width, int height)
    {
        ThrowIfDisposed();
        
        if (_renderContext == nint.Zero)
            throw new InvalidOperationException("render context is not initialized");
        
        if(width <= 0 || height <= 0)
            return;
        
        nint fboPtr = nint.Zero;
        nint flipYPtr = nint.Zero;
        nint paramsPtr = nint.Zero;

        try
        {
            // описываем целевой FBO
            var fbo = new MpvNative.MpvOpenGlFbo
            {
                Fbo = framebuffer,
                Width = width,
                Height = height,
                InternalFormat = 0
            };

            fboPtr = AllocStruct(fbo);

            // если видео будет перевернуто, поставить 1
            var flipY = 1;
            flipYPtr = AllocStruct(flipY);

            // параметры для mpv_render_context_render()
            var renderParams = new[]
            {
                new MpvNative.MpvRenderParam
                {
                    Type = MpvNative.MpvRenderParamType.OpenGlFbo,
                    Data = fboPtr,
                },
                new MpvNative.MpvRenderParam
                {
                    Type = MpvNative.MpvRenderParamType.FlipY,
                    Data = flipYPtr,
                },
                new MpvNative.MpvRenderParam
                {
                    Type = MpvNative.MpvRenderParamType.Invalid,
                    Data = nint.Zero,
                }
            };
            paramsPtr = AllocArray(renderParams);

            var code = MpvNative.mpv_render_context_render(_renderContext, paramsPtr);
            ThrowIfMpvError(code, "mpv_render_context_render");
        }
        finally
        {
            FreeIfNotZero(fboPtr);
            FreeIfNotZero(flipYPtr);
            FreeIfNotZero(paramsPtr);
        }
    }

    public void Dispose()
    {
        if(_isDisposed) return;

        // отключаем колбек чтобы mpv не дергал уже освобожденный объект
        if (_renderContext != nint.Zero)
        {
            MpvNative.mpv_render_context_set_update_callback(_renderContext, nint.Zero, nint.Zero);
            MpvNative.mpv_render_context_free(_renderContext);
            _renderContext = nint.Zero;
        }
        
        // освобождаем GCHandle
        if(_selfHandle.IsAllocated)
            _selfHandle.Free();
        
        _getProcAddress = null;
        _getProcAddressCallback = null;
        _updateCallback = null;
        
        _isDisposed = true;
    }
    
    // trampoline: mpv -> нас Func<string, nint>
    private static nint GetProcAddressTrampoline(nint ctx, string name)
    {
        var handle = GCHandle.FromIntPtr(ctx);
        var bridge = (MpvRenderBridge?)handle.Target;

        if (bridge?._getProcAddress is null)
            return nint.Zero;

        return bridge._getProcAddress(name);
    }
    
    // trampoline: mpv сообщает что нужен редров
    private static void UpdateTrampoline(nint ctx)
    {
        var handle = GCHandle.FromIntPtr(ctx);
        var bridge = (MpvRenderBridge?)handle.Target;
        bridge?._requestRedraw();
    }
    
    private void ThrowIfMpvError(int code, string operation)
    {
        if (code >= 0) return;

        var errorPtr = MpvNative.mpv_error_string(code);
        var errorText = Marshal.PtrToStringUTF8(errorPtr) ?? $"unknown mpv error {code}";
        
        throw new InvalidOperationException($"{operation} failed: {errorText} ({code})"); 
    }
    
    private void ThrowIfDisposed()
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(MpvRenderBridge));
    }
    
    // helper: кладем структуру в анманеджет память
    private static nint AllocStruct<T>(T value) where T : struct
    {
        var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<T>());
        Marshal.StructureToPtr(value, ptr, false);
        return ptr;
    }
    
    // helper: кладем массив структур в анманаджет память
    private static nint AllocArray<T>(T[] items) where T : struct
    {
        var size = Marshal.SizeOf<T>();
        var ptr = Marshal.AllocHGlobal(size * items.Length);

        for (int i = 0; i < items.Length; i++)
        {
            var itemPtr = ptr + i * size;
            Marshal.StructureToPtr(items[i], itemPtr, false);
        }
        
        return ptr;
    }

    private static void FreeIfNotZero(nint ptr)
    {
        if (ptr != nint.Zero)
            Marshal.FreeHGlobal(ptr);
    }
}