namespace ExamPaper.Core.Generation;

public interface IGenerationSettings
{
    uint TotalTicketsCount { get; }
    uint QuestionsPerTicketCount { get; }
}