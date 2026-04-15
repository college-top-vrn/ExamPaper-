using System;

namespace ExamPaper.Core.Interfaces;

public interface IQuestion
{
    Guid Id { get; init; }
    string Text { get; init; }

}
