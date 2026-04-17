namespace ExamPaper.Core.Interfaces;

using System;
using System.Collections.Generic;

/// <summary>
/// Интерфейс экзаменационного билета
/// </summary>
public interface IExamPaper
{
    /// <summary>
    /// Уникальный Guid идентификатор билета.
    /// </summary>
    Guid Id { get; init; }

    /// <summary>
    /// Название билета.
    /// </summary>
    string Title { get; init; }

    /// <summary>Список вопросов, попавших в данный билет.</summary>
    IReadOnlyCollection<IQuestion> Questions { get; init; }
}