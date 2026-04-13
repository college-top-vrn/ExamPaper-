namespace ExamPaper.Core.Interfaces;

public interface IQuestion
{
    Guid Id { get; }
    string Text { get; }
}
