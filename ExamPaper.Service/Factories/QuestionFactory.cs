using System;

using ExamPaper.Core.Interfaces;
using ExamPaper.Core.Models;

namespace ExamPaper.Service.Factories;

public class QuestionFactory : IQuestionFactory
{
    public IQuestion CreateQuestion(Guid id, string text) => new Question(id, text); 



}
