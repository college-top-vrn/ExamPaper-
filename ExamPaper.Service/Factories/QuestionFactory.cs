using System;
using ExamPaper.Core.Interfaces;
using ExamPaper.Core.Models;

namespace ExamPaper.Service.Factories;

/// <summary>
/// фабрика для создания объекта Question
/// </summary>
public class QuestionFactory : IQuestionFactory
{
    /// <summary>
    /// метод для созздания обекта Question, где создается новый объект Question
    /// </summary>
    /// <param name="id">id вопроса</param>
    /// <param name="text"> текст вопроса</param>
    /// <returns></returns>
    public IQuestion CreateQuestion(Guid id, string text) => new Question(id, text);
}
