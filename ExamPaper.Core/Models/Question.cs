using System;
using System.Text.Json.Serialization;

using ExamPaper.Core.Interfaces;

namespace ExamPaper.Core.Models;

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
    /// Конструктор для создания экзаменационного вопроса с автоматической генерацией Id (Version 7).
    /// </summary>
    /// <param name="text">Текст вопроса.</param>
    /// <exception cref="ArgumentException">Если текст пуст.</exception>
    public Question(string text)
        : this(Guid.CreateVersion7(), text) { }

    /// <summary>
    /// Конструктор для создания экзаменационного вопроса с заданным Id.
    /// </summary>
    /// <param name="id">Уникальный Guid идентификатор вопроса.</param>
    /// <param name="text">Текст вопроса.</param>
    /// <exception cref="ArgumentException">Если текст пуст.</exception>
    [JsonConstructor]
    public Question(Guid id, string text)
    {
        Id = id == Guid.Empty ? Guid.CreateVersion7() : id;

        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Текст вопроса не может быть пустым.", nameof(text));

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