using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Pika.Playback.Mpv.Interop;

internal static partial class MpvNative
{
    // FROM /usr/include/mpv/render.h
    internal enum MpvRenderParamType
    {
        Invalid = 0,
        ApiType = 1,
        OpenGlInitParams = 2,
        OpenGlFbo = 3,
        FlipY = 4
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MpvRenderParam
    {
        public MpvRenderParamType Type;
        public nint Data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MpvOpenGlInitParams
    {
        public nint GetProcAddress;
        public nint GetProcAddressCtx;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MpvOpenGlFbo
    {
        public int Fbo;
        public int Width;
        public int Height;
        public int InternalFormat;
    }
    
    // new render
    [LibraryImport("libmpv.so.2")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int mpv_render_context_create(out nint renderContext, nint mpv, nint parameters);
    
    [LibraryImport("libmpv.so.2")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial ulong mpv_render_context_update(nint renderContext);

    [LibraryImport("libmpv.so.2")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int mpv_render_context_render(nint renderContext, nint parameters);
    
    [LibraryImport("libmpv.so.2")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void mpv_render_context_free(nint renderContext);
    
    // FROM /usr/include/mpv/client.h
    [LibraryImport("libmpv.so.2")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial nint mpv_create();

    [LibraryImport("libmpv.so.2")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int mpv_initialize(nint ctx);
    
    [LibraryImport("libmpv.so.2")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void mpv_destroy(nint ctx);
    
    [LibraryImport("libmpv.so.2",
        StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int mpv_set_option_string(nint ctx, string option, string value);
    
    [LibraryImport("libmpv.so.2",
        StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int mpv_command_string(nint ctx, string args);
    
    [LibraryImport("libmpv.so.2")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial nint mpv_error_string(int error);
    
        
}