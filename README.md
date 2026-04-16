## Диаграмма компонентов
![Диаграмма компонентов](docs/assets/diagram_component.SVG)

## Регламент
- ![Создание ветки](docs/BranchCreation.md);
- ![Создание файлов и директорий](docs/FileAndDirectoriesCreation.md);
- ![Проектирование классов](docs/ClassPlanning.md).

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
