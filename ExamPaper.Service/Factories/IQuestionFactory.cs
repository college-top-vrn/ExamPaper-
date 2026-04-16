using System;

using ExamPaper.Core.Interfaces;
using ExamPaper.Core.Models;

namespace ExamPaper.Service.Factories;

public interface IQuestionFactory
{
    IQuestion CreateQuestion(Guid id, string text);
}