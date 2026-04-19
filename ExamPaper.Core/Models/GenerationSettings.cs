using System;

using ExamPaper.Core.Interfaces;

namespace ExamPaper.Core.Models;

/// <summary>
///     Реализация настроек генерации экзаменационных билетов.
/// </summary>
public sealed class GenerationSettings : IGenerationSettings
{
    /// <summary>
    ///     Конструктор для создания настроек с параметрами.
    /// </summary>
    /// <param name="totalTicketsCount">Общее количество билетов.</param>
    /// <param name="questionsPerTicketCount">Количество вопросов на билет.</param>
    /// <exception cref="ArgumentException">Если параметры некорректны.</exception>
    public GenerationSettings(int totalTicketsCount, int questionsPerTicketCount)
    {
        if (totalTicketsCount <= 0)
        {
            throw new ArgumentException(
                "Общее количество билетов должно быть положительным",
                nameof(totalTicketsCount)
            );
        }

        if (questionsPerTicketCount <= 0)
        {
            throw new ArgumentException(
                "Количество вопросов на билет должно быть положительным",
                nameof(questionsPerTicketCount)
            );
        }

        TotalTicketsCount = totalTicketsCount;
        QuestionsPerTicketCount = questionsPerTicketCount;
    }

    /// <summary>
    ///     Пустой конструктор для десериализации (JSON/XML).
    /// </summary>
    public GenerationSettings() { }

    /// <summary>
    ///     Общее количество билетов.
    /// </summary>
    public int TotalTicketsCount { get; }

    /// <summary>
    ///     Количество вопросов в одном билете.
    /// </summary>
    public int QuestionsPerTicketCount { get; }

    /// <summary>
    ///     Вспомогательный метод для валидации настроек.
    /// </summary>
    public bool IsValid()
    {
        return TotalTicketsCount > 0 && QuestionsPerTicketCount > 0;
    }

    /// <summary>
    ///     Переопределенный метод, выводящий информацию о настройках билета.
    /// </summary>
    /// <returns>
    ///     Строка с информацией о настройках билета.
    /// </returns>
    public override string ToString()
    {
        return $"Билетов: {TotalTicketsCount}, Вопросов на билет: {QuestionsPerTicketCount}";
    }
}