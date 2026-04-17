using ExamPaper.Core.Interfaces;

namespace ExamPaper.Core.Models;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

/// <summary>
/// Класс для хранения экзаменационного билета.
/// </summary>
public sealed class ExamPaper : IExamPaper
{
    /// <summary>
    /// Уникальный Guid идентификатор билета.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Название билета.
    /// </summary>
    public string Title { get; init; }

    /// <summary>
    /// Список вопросов, попавших в данный билет.
    /// </summary>
    public IReadOnlyCollection<IQuestion> Questions { get; init; }

    /// <summary>
    /// Конструктор для создания билета с автоматической генерацией Id (Version 7).
    /// </summary>
    /// <param name="title">Название билета.</param>
    /// <param name="questions">Список вопросов, попавших в данный билет.</param>
    /// <exception cref="ArgumentNullException">Если параметры пусты.</exception>
    public ExamPaper(string title, IEnumerable<IQuestion> questions)
        : this(Guid.CreateVersion7(), title, questions) { }

    /// <summary>
    /// Конструктор для создания экзаменационного билета с заданным Id.
    /// </summary>
    /// <param name="id">Уникальный Guid идентификатор билета.</param>
    /// <param name="title">Название билета.</param>
    /// <param name="questions">Список вопросов, попавших в данный билет.</param>
    /// <exception cref="ArgumentNullException">Если список вопросов пуст.</exception>
    public ExamPaper(Guid id, string title, IEnumerable<IQuestion> questions)
    {
        Id = id == Guid.Empty ? Guid.CreateVersion7() : id;

        Title = title ?? throw new ArgumentNullException(nameof(title));
        Questions = new ReadOnlyCollection<IQuestion>(
            (questions ?? throw new ArgumentNullException(nameof(questions))).ToList()
        );
    }
}