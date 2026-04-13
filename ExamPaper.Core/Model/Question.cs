using ExamPaper.Core.Interfaces;

namespace ExamPaper.Core.Model;

public sealed class Question : IQuestion
{
    public Guid Id { get; }
    public string Text { get; }

    public Question(Guid id, string text)
    {
        Id = id;
        Text = text;
        Validate();
        
    }
    public void Validate()
    {
        if (Id == Guid.Empty)
            throw new ArgumentException("Id must be non-empty.", nameof(Id));

        if (string.IsNullOrWhiteSpace(Text))
            throw new ArgumentException("Text must be filled.", nameof(Text));
    }
}