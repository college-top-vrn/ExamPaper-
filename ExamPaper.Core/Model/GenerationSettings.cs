using System;

using ExamPaper.Core.Generation;

namespace ExamPaper.Core.Model;

public class GenerationSettings : IGenerationSettings
{
    public int TotalTicketsCount { get; }
    public int QuestionsPerTicketCount { get; }

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
    
    public GenerationSettings() { }

    public bool IsValid() => TotalTicketsCount > 0 && QuestionsPerTicketCount > 0;

    public override string ToString() 
        => $"Билетов: {TotalTicketsCount}, Вопросов на билет: {QuestionsPerTicketCount}";

}