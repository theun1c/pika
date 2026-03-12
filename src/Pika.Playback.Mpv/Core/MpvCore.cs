using System.Runtime.InteropServices;
using Pika.Playback.Mpv.Interop;

namespace Pika.Playback.Mpv.Core;

// обертка над MpvNative
public sealed class MpvCore : IDisposable
{
    private nint _handle; // нативный указатель на mpv_handle*
    private bool _isInitialized; // флаг то что mpv_initialize уже вызван
    private bool _isDisposed; // флаг, что объект уже освобожден
    
    public nint Handle => _handle; // отдаем render слою 
    
    // точка старта lifecycle
    public void Initialize()
    {
        ThrowIfDisposed(); // запрет работы после dispose
        if (_isInitialized) return; // игнорируем повторный init

        _handle = MpvNative.mpv_create(); // создаем core mpv
        if (_handle == nint.Zero) // проверка то что не 0
            throw new InvalidOperationException("mpv_create() failed. returned null");

        SetOption("vo", "libmpv");
        
        var initResult = MpvNative.mpv_initialize(_handle);
        ThrowIfMpvError(initResult, "mpv_initialize");
        
        _isInitialized = true;
    }

    // загрузка видео
    public void Load(string source)
    {
        ThrowIfDisposed();
        if(!_isInitialized) // требуем сначала инициализировать 
            throw new InvalidOperationException("call initialize first");
        
        // проверка на пустую строку
        if(string.IsNullOrEmpty(source))
            throw new ArgumentNullException("source is empty",nameof(source));
        
        // экранируем кавычки
        var escaped = source.Replace("\"","\\\"");
        var command = $"loadfile \"{escaped}\""; // команда для загрузки
        var code = MpvNative.mpv_command_string(_handle, command); // вызываем команды и получаем код вызова
        ThrowIfMpvError(code, "mpv_command_string(loadfile)"); // если ошибка 
    }

    // управление паузой
    public void SetPause(bool pause)
    {
        ThrowIfDisposed();
        if(!_isInitialized)
            throw new InvalidOperationException("call initialize first");
        
        var command = pause ? "set pause yes" : "set pause no";
        var code = MpvNative.mpv_command_string(_handle, command);
        ThrowIfMpvError(code, "mpv_command_string(set pause)");
    }
    
    // освобождение
    public void Dispose()
    {
        if(_isDisposed) return;

        // удаление 
        if (_handle != nint.Zero)
        {
            MpvNative.mpv_destroy(_handle);
            _handle = nint.Zero;
        }
        
        // изменение флагов
        _isInitialized = false;
        _isDisposed = true;
        // финализация (освобождение неуправляемых ресурсов)
        GC.SuppressFinalize(this);
    }

    // установка опций
    public void SetOption(string option, string value)
    {
        ThrowIfDisposed();
        if (_handle == nint.Zero)
            throw new InvalidOperationException("call initialize first");
        
        var code = MpvNative.mpv_set_option_string(_handle, option, value);
        ThrowIfMpvError(code, $"mpv_set_option_string(_handle, {option}, {value})");
    }

    // метод для ошибок
    private void ThrowIfMpvError(int code, string opetarion)
    {
        if (code >= 0) return;

        var errorPtr = MpvNative.mpv_error_string(code);
        var errorText = Marshal.PtrToStringUTF8(errorPtr) ?? $"unknown mpv error {code}";
        
        throw new InvalidOperationException($"{opetarion} failed: {errorText} ({code})"); 
    }
    
    // метод если уже освобожден
    private void ThrowIfDisposed()
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(MpvCore));
    }

}