using ExamPaper.Core.Interfaces;

namespace ExamPaper.Core.Models;
using System;
using System.Linq;

public sealed class Question : IQuestion
{
    public Guid Id { get; }
    public string Text { get; }

    public Question(Guid id, string text)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("id вопроса не может быть пустым", nameof(id));

        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("текст вопроса не может быть пустым.", nameof(text));

        Id = id;
        Text = text.Trim();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Question other)
            return false;

        return Id == other.Id && Text == other.Text;
    }

    public override int GetHashCode() => HashCode.Combine(Id, Text);
}