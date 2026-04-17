using System;

namespace ExamPaper.Core.Interfaces;

/// <summary>
///     Интерфейс фабрики создания вопросов QuestionFactory
/// </summary>
public interface IQuestionFactory
{
    /// <summary>
    ///     метод создания вопросов
    /// </summary>
    /// <param name="id">id вопроса</param>
    /// <param name="text">текст вопроса </param>
    /// <returns></returns>
    IQuestion CreateQuestion(Guid id, string text);
}