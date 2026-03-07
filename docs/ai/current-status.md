# Текущий статус проекта

Проект: Pika

Стадия разработки: начальная.

---

# Что уже сделано

- создан Git repository
- написан README
- создан базовый Avalonia Desktop проект
- описана предварительная архитектура проекта

---

# Что пока не реализовано

- API (ASP.NET Core)
- Worker service
- Database schema
- Domain model
- Source connectors
- Playback система

---

# Текущий фокус разработки

Сейчас основная задача — **проектирование архитектуры и структуры проекта**.

Ближайшие цели:

1. определить структуру solution
2. описать domain entities
3. создать API проект
4. добавить database layer
5. реализовать первый source connector
6. подключить desktop клиент к API

---

# Важные архитектурные правила

- Desktop **не должен** парсить сайты
- Catalog хранится на backend
- Connectors реализуются как отдельные модули
- Worker выполняет тяжелые операции с external sources
- Playback ссылки должны определяться **on-demand**
- Domain model должна быть независимой от UI и Infrastructure

---

# Следующая задача разработки

Создать структуру solution со следующими проектами:

- API
- Worker
- Core / Domain
- Infrastructure
- Connectors

После этого начать реализацию **domain model**.