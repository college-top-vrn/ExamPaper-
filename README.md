# Диаграммы
## Компонентов
```mermaid
flowchart TD
    subgraph CoreLayer [Слой ядра]
        Models[Models]
        Interfaces[Interfaces]
    end

    subgraph ServiceLayer [Слой сервисов]
        ExamPaperGenerator[ExamPaper Generator]
        Factory[Factory]
    end

    subgraph InfrastructureLayer [Слой инфраструктуры]
        Exporter[Exporter]
        Repository[Repository]
    end

    ExamPaperGenerator --> Factory
    ExamPaperGenerator --> Repository

    Factory --> Models
    Factory --> Interfaces

    Exporter --> Models
```

## Прецедентов
```mermaid
flowchart LR
  actor1[Пользователь]


    uc1((Добавление списка вопросов))
    uc2((Выбирает количество билетов и вопросов))
    uc3((Получает файл с билетами в PDF формате ))


  actor1 --> uc1
  uc1 --> uc2
  uc2 --> uc3
```
## Последовательности
```mermaid
sequenceDiagram
    participant Пользователь
    participant Система

    Пользователь->>Система: Добавляет список вопросов
    Пользователь->>Система: Указывает количество вопросов в билете
    Пользователь->>Система: Указывает количество билетов
    Система->>Система: Генерирует билеты
    Система->>Пользователь: Выдает PDF‑файл с билетами
```

## Регламент
- ![Создание ветки](rules/BranchCreation.md);
- ![Создание файлов и директорий](rules/FileAndDirectoriesCreation.md).
- ![Создание тест-кейсов](rules/TestCaseRegulations.md)

## Цель проекта: Разработка генератора экзаменационных билетов.

Ключевые функции:

- Хранение вопросов и билетов в JSON-файле.
- Генерация билетов по запросу пользователя (количество вопросов варьируется с количеством билетов).
- Экспорт в PDF (используя библиотеку QuestPDF)

## Контракты базовых сущностей
```cs
namespace ExamPaper.Core.Interfaces
{
    public interface IQuestion
    {
   
        Guid Id { get; }
        string Text { get; }
    }
}
```

```cs
using System;
using System.Collections.Generic;

namespace ExamPaper.Core.Interfaces
{
  
    public interface IExamPaper
    {

        Guid Id { get; }
        string Title { get; }
        IReadOnlyCollection<IQuestion> Questions { get; }

    }
}
```
