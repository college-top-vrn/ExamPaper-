using System;
using System.Collections.Generic;

namespace ExamPaper.Core.Interfaces;

/// <summary>
/// интервейс фбрики ExamPaper
/// </summary>
public interface IExamPaperFactory
{
    /// <summary>
    /// метод для создания билета
    /// </summary>
    /// <param name="id">id билета</param>
    /// <param name="title">имя билета</param>
    /// <param name="questions">списовк вопросов для создания билета </param>
    /// <returns></returns>
    IExamPaper CreateExamPaper(Guid id, string title, IEnumerable<IQuestion> questions);
}