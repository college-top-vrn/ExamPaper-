namespace ExamPaper.Core.Generation;

/// <summary>
/// Контракт для хранения параметров генерации билетов.
/// </summary>
public interface IGenerationSettings
{
    /// <summary>
    /// Получение общего количества билетов, которые необходимо создать.
    /// </summary>
    /// <value>Положительное целое число.</value>
    int TotalTicketsCount { get; }
    /// <summary>
    /// Получение количества вопросов, закрепленных за одним билетом.
    /// </summary>
    /// <value>Положительное целое число.</value>
    int QuestionsPerTicketCount { get; }
}