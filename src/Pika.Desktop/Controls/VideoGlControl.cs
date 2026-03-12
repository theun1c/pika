using System;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Threading;
using Pika.Playback.Mpv.Core;

namespace Pika.Desktop.Controls;

// OpenGL-контрол, который внутри себя держит mpv core + render bridge
public sealed class VideoGlControl : OpenGlControlBase
{
    // lifecycle плеера и команд 
    private MpvCore? _core;
    
    // render_context и отрисовка кадра в FBO
    private MpvRenderBridge? _bridge;

    // если source зададут до OnOpenGlInit временно храним его здесь
    private string? _pendingSource;
    
    // публичный метод. UI модет передать url в любой момент
    public void SetSource(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
            throw new ArgumentException("source is empty", nameof(source));
        
        _pendingSource = source;
        
        // если core уже поднят можно сразу грузить 
        if (_core is not null)
        {
            _core.Load(source);
            
            // просим перерисовку кадра
            RequestNextFrameRendering();
        }
    }
    
    // вызываем когда у контрола появился рабочий GL context
    protected override void OnOpenGlInit(GlInterface gl)
    {
        base.OnOpenGlInit(gl);
        
        // если init вызвали повторно, сначала почистим старые объекты
        _bridge?.Dispose();
        _bridge = null;
        
        _core?.Dispose();
        _core = null;
        
        // создаем и инициализируем mpv code
        _core = new MpvCore();
        _core.Initialize();
        
        // создаем бридж и передаем колбек для реквест редро
        // колбек может прилететь не из UI -> постим в диспатчер
        _bridge = new MpvRenderBridge(() =>
        {
            Dispatcher.UIThread.Post(RequestNextFrameRendering);
        });
        
        // передаем handle mpv core + функцию резолва GL-символов
        _bridge.InitializeFromOpenGl(_core.Handle, name => gl.GetProcAddress(name));
        
        // если source был задан заранее - грузим его сейчас
        if(!string.IsNullOrWhiteSpace(_pendingSource))
            _core.Load(_pendingSource);
        
        // стартовый кадр
        RequestNextFrameRendering();
    }
    
    // вызывается на GL-потоке каждый requested frame
    protected override void OnOpenGlRender(GlInterface gl, int framebuffer)
    {
        // если еще не инициализировано — выходим
        if (_bridge is null)
            return;

        // размеры целевой поверхности (минимум 1x1)
        var width = Math.Max(1, (int)Bounds.Width);
        var height = Math.Max(1, (int)Bounds.Height);

        // update() сообщает, есть ли новый кадр
        if (_bridge.Update())
            _bridge.Render(framebuffer, width, height);
    }

    protected override void OnOpenGlDeinit(GlInterface gl)
    {
        base.OnOpenGlDeinit(gl);
        
        // освобождаем render bridge
        _bridge?.Dispose();
        _bridge = null;
        
        // освобождаем mpv core
        _core?.Dispose();
        _core = null;
    }
}