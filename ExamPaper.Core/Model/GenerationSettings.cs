using System;

using ExamPaper.Core.Generation;

namespace ExamPaper.Core.Model;

/// <summary>
/// Реализация настроек генерации экзаменационных билетов.
/// </summary>
public class GenerationSettings : IGenerationSettings
{
    /// <summary>
    /// Общее количество билетов.
    /// </summary>
    public int TotalTicketsCount { get; }
    /// <summary>
    /// Количество вопросов в одном билете.
    /// </summary>
    public int QuestionsPerTicketCount { get; }
    
    /// <summary>
    /// Конструктор для создания настроек с параметрами.
    /// </summary>
    /// <param name="totalTicketsCount">Общее количество билетов</param>
    /// <param name="questionsPerTicketCount">Количество вопросов на билет</param>
    /// <exception cref="ArgumentException">Если параметры некорректны</exception>
    public GenerationSettings(int totalTicketsCount, int questionsPerTicketCount)
    {
        if (totalTicketsCount <= 0)
            throw new ArgumentException("Общее количество билетов должно быть положительным",
                nameof(totalTicketsCount));
        if (questionsPerTicketCount <= 0)
            throw new ArgumentException("Количество вопросов на билет должно быть положительным",
                nameof(questionsPerTicketCount));
        
        TotalTicketsCount = totalTicketsCount;
        QuestionsPerTicketCount = questionsPerTicketCount;
    }
    /// <summary>
    /// Пустой конструктор для десериализации (JSON/XML).
    /// </summary>
    public GenerationSettings() { }
    /// <summary>
    /// Вспомогательный метод для валидации настроек.
    /// </summary>
    public bool IsValid() => TotalTicketsCount > 0 && QuestionsPerTicketCount > 0;

    public override string ToString() 
        => $"Билетов: {TotalTicketsCount}, Вопросов на билет: {QuestionsPerTicketCount}";

}