# Записки разработчика 

# Разработка плеера с libmpv бекендом 

Упрощенный flow
1. Проверка окружения Linux
• Нужно, чтобы libmpv был установлен и доступен в системе.
• Проверка: mpv работает в терминале и libmpv виден линкеру.
2. Smoke test interop (без UI)
• Сделай маленький вызов mpv_client_api_version() из C#.
• Если это не работает, дальше не идем.
``` C#
[LibraryImport("mpv")]
public static partial ulong mpv_client_api_version();
```
3. Пустой GL-контрол в Avalonia

• Создай OpenGlControlBase, который рисует черный фон.

• Это проверка, что рендер-поверхность в окне живая.

4. Подключение mpv core

• mpv_create -> опции -> mpv_initialize.

• Пока без render API, просто убедись, что объект плеера создается.

5. Связка с mpv_render_context

• В OnOpenGlInit создаешь render context.

• В OnOpenGlRender вызываешь mpv_render_context_render.

• В callback обновления вызываешь RequestNextFrameRendering().

6. Загрузка источника

• Сначала локальный mp4.

• Потом твой m3u8.

• Если m3u8 не играет, проверяешь заголовки (referrer, user-agent, cookies).

7. Минимальный API для UI

• Достаточно одного метода: SetSource(string urlOrPath).

• Кнопки потом.

``` C#
public interface IVideoPlayerHost
{
    void SetSource(string source);
}
```

Минимальная структура файлов

• Interop/MpvNative.cs — только P/Invoke

• Playback/MpvCore.cs — lifecycle и команды mpv

• Controls/VideoGlControl.cs — Avalonia OpenGL + render callback

Definition of Done (этап 1)
1. В окне Avalonia есть видео-зона.
2. Локальный mp4 воспроизводится внутри этой зоны.
3. m3u8 воспроизводится внутри этой зоны.
4. Resize окна не ломает рендер.
5. При закрытии окна нет крашей/утечек.

TODO 
1. Проверить наличие libmpv в Linux.
2. Сделать interop smoke test с mpv_client_api_version.
3. Сделать черный OpenGlControlBase.
4. Добавить mpv_create/mpv_initialize.
5. Подключить mpv_render_context в GL lifecycle.
6. Прогнать локальный mp4.
7. Прогнать m3u8.
8. Зафиксировать ошибки m3u8 в логах (если будут).