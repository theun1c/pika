# Pika — Контекст проекта (для AI)

Pika — это персональная платформа-агрегатор для просмотра аниме, создаваемая на .NET.

Цель проекта — собирать данные об аниме и доступных вариантах просмотра из разных источников и объединять их в единый интерфейс.

Проект находится на ранней стадии разработки.

---

# Основная идея

Приложение должно выполнять две основные задачи.

## 1. Catalog

Определить:

- какие anime существуют
- какие у них installments (season / movie / OVA / special)
- какие есть episodes
- какие есть metadata

Catalog описывает **то, что существует**.

---

## 2. Playback

Определить:

- где episode можно посмотреть
- какие есть quality options
- какие есть dub / subtitles
- как получить playback stream

Playback описывает **то, что можно воспроизвести прямо сейчас**.

---

# Важный архитектурный принцип

Catalog data и Playback data — **разные типы данных**.

Catalog может храниться долго.

Playback должен определяться **on-demand**, когда пользователь выбирает episode.

---

# Планируемая архитектура (черновая)

В будущем проект будет состоять из следующих компонентов:

Desktop Client (Avalonia)  
API (ASP.NET Core)  
Worker (background service)  
Database  
Source Connectors

Высокоуровневая схема:

```
User  
↓  
Desktop UI  
↓  
API  
↓  
Database  
↓  
Worker  
↓  
Source Connectors  
↓  
External Sources
```

Но эта архитектура **пока не реализована полностью**.

Сейчас проект находится в исследовательской фазе.

---

# Исследовательская фаза (Sandbox)

На текущем этапе задача — изучить источники данных и понять структуру информации.

Для этого используется **Sandbox подход**.

Sandbox позволяет:

- экспериментировать с API и HTML страницами
- тестировать парсинг
- проверять модели данных
- понять структуру источников

Sandbox код **не считается финальной архитектурой**.

После исследования он будет разделен на правильные слои.

---

# Основные компоненты проекта (план)

## Desktop

Avalonia UI приложение.

Отвечает за:

- search UI
- страницы anime
- список episodes
- выбор playback
- запуск player
- watch history

Desktop **не должен**:

- парсить сайты напрямую
- хранить основной catalog
- выполнять тяжелую синхронизацию

---

## API

Центральный orchestration слой.

Отвечает за:

- API endpoints
- выдачу catalog данных
- управление playback
- взаимодействие с database

---

## Worker

Background service.

Отвечает за:

- сбор данных из sources
- обновление catalog
- синхронизацию episodes
- background jobs

---

## Source Connectors

Адаптеры для конкретных external sources.

Каждый connector умеет:

- search titles
- получать страницу anime
- получать список episodes
- получать playback options
- resolve playback session

---

# Domain Model (предварительный)

Сущности пока не финализированы.

Предварительно предполагаются:

Anime  
Installment  
Episode  
SourceLink  
EpisodeOffer  
WatchHistory

Финальная Domain model будет определена **после исследования источников**.

---

# Используемые технологии

.NET  
C#  
ASP.NET Core  
Avalonia UI  
Entity Framework Core  
SQLite / PostgreSQL