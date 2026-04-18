using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using ExamPaper.Core.Interfaces;

namespace ExamPaper.Core.Models;

/// <summary>
///     Класс для хранения экзаменационного билета.
/// </summary>
public sealed class ExamPaper : IExamPaper
{
    /// <summary>
    ///     Конструктор для создания билета с автоматической генерацией Id (Version 7).
    /// </summary>
    /// <param name="title">Название билета.</param>
    /// <param name="questions">Список вопросов, попавших в данный билет.</param>
    /// <exception cref="ArgumentNullException">Если параметры пусты.</exception>
    public ExamPaper(string title, IEnumerable<IQuestion> questions)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(questions);

        Id = Guid.CreateVersion7();
        Title = title;
        Questions = new ReadOnlyCollection<IQuestion>(questions.ToList());
    }

    /// <summary>
    ///     Конструктор для создания экзаменационного билета с заданным Id.
    /// </summary>
    /// <param name="id">Уникальный Guid идентификатор билета.</param>
    /// <param name="title">Название билета.</param>
    /// <param name="questions">Список вопросов, попавших в данный билет.</param>
    /// <exception cref="ArgumentNullException">Если список вопросов пуст.</exception>
    public ExamPaper(Guid id, string title, IEnumerable<IQuestion> questions)
    {
        Id = id == Guid.Empty ? Guid.CreateVersion7() : id;

        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(questions);

        Title = title;
        Questions = questions.ToList().AsReadOnly();
    }

    /// <summary>
    ///     Уникальный Guid идентификатор билета.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    ///     Название билета.
    /// </summary>
    public string Title { get; init; }

    /// <summary>
    ///     Список вопросов, попавших в данный билет.
    /// </summary>
    public IReadOnlyCollection<IQuestion> Questions { get; init; }
}

