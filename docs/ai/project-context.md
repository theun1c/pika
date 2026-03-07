# Pika — Контекст проекта (для AI)

Pika — это персональная платформа-агрегатор для просмотра аниме, создаваемая на экосистеме .NET.

Цель проекта — собирать информацию об аниме и доступных вариантах просмотра из разных источников и объединять их в едином интерфейсе.

Проект находится на ранней стадии разработки.

---

# Основная идея

Система разделяет **catalog data** и **playback data**.

### Catalog data — описывает то, что существует

- anime titles
- installments (season / movie / OVA / special)
- episodes
- metadata

### Playback data — описывает то, что можно посмотреть прямо сейчас

- available sources
- quality options
- dubbing / subtitles
- playback streams

Эти два типа данных **нельзя смешивать**.

Catalog может храниться долго.  
Playback должен определяться **по запросу**.

---

# Планируемая архитектура

Проект проектируется как **modular monolith**, состоящий из нескольких приложений.

Основные компоненты:

Desktop Client  
API  
Worker  
Database  
Source Connectors

Общая схема системы:
``` 
User
↓
Desktop UI (Avalonia)
↓
API (ASP.NET Core)
↓
Database
↓
Worker
↓
Source Connectors
↓
External Sources
```


---

# Ответственности компонентов

## Desktop

Пользовательский интерфейс.

Отвечает за:

- search UI
- страницы аниме
- список episodes
- интерфейс выбора playback
- watch history
- запуск player

Desktop **не должен**:

- парсить сайты
- хранить основной catalog
- выполнять тяжелую синхронизацию данных

Desktop — это **UI слой**, а не центр логики.

---

## API

Центральный orchestration слой.

Отвечает за:

- API endpoints
- выдачу catalog данных
- взаимодействие с database
- запуск background jobs
- управление playback sessions

API является **основной точкой взаимодействия клиентов с системой**.

---

## Worker

Background service.

Отвечает за:

- получение данных из sources
- обновление catalog
- обновление episodes
- проверку доступности sources
- выполнение background jobs

Worker выполняет тяжелые операции вне API.

---

## Source Connectors

Адаптеры для конкретных external sources.

Каждый connector умеет:

- search titles
- получать страницу anime
- получать список episodes
- получать playback options
- resolve playback session

Каждый source должен быть **изолированным модулем**.

---

# Domain Model (черновой вариант)

Основные сущности:

Anime  
Installment  
Episode  
SourceLink  
EpisodeOffer  
WatchHistory

Domain model — это **общий язык системы**, используемый всеми компонентами.

---

# Используемые технологии

.NET  
C#  
ASP.NET Core  
Avalonia UI  
Entity Framework Core  
SQLite / PostgreSQL

---

# Философия разработки

- модульная архитектура
- разделение ответственности компонентов
- connectors изолированы от core логики
- UI должен оставаться thin client
- backend владеет catalog данными
- playback данные определяются on-demand