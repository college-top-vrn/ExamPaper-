using System;

namespace ExamPaper.Core.Interfaces;

/// <summary>
///     Интерфейс экзаменационного вопроса.
/// </summary>
public interface IQuestion
{
    /// <summary>
    ///     Уникальный Guid идентификатор вопроса.
    /// </summary>
    Guid Id { get; init; }

    /// <summary>
    ///     Текст вопроса.
    /// </summary>
    string Text { get; init; }
}