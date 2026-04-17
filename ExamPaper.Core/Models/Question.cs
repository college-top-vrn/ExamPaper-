using ExamPaper.Core.Interfaces;

namespace ExamPaper.Core.Models;

using System;

/// <summary>
/// Класс для хранения экзаменационного вопроса
/// </summary>
public sealed class Question : IQuestion
{
    /// <summary>
    /// Уникальный Guid идентификатор вопроса.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Текст вопроса.
    /// </summary>
    public string Text { get; init; }

    /// <summary>
    /// Конструктор для создания экзаменационного вопроса с параметрами.
    /// </summary>
    /// <param name="id">Уникальный Guid идентификатор вопроса.</param>
    /// <param name="text">Текст вопроса.</param>
    /// <exception cref="ArgumentException">Если один из параметров пуст.</exception>
    public Question(Guid id, string text)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id вопроса не может быть пустым.", nameof(id));

        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Текст вопроса не может быть пустым.", nameof(text));

        Id = id;
        Text = text.Trim();
    }

    /// <summary>
    /// Переопределенный метод для сравнения одного вопроса с другим вопросом.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>Результат сравнения</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not Question other)
            return false;

        return Id == other.Id && Text == other.Text;
    }

    /// <summary>
    /// Метод для получения хэш-кода вопроса.
    /// </summary>
    /// <returns>Хэш-код вопроса.</returns>
    public override int GetHashCode() => HashCode.Combine(Id, Text);
}
