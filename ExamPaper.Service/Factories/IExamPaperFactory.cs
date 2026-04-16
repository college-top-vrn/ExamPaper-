using System;
using System.Collections.Generic;

using ExamPaper.Core.Interfaces;

namespace ExamPaper.Service.Factories;

/// <summary>
/// интервейс фбрики ExamPaper
/// </summary>
public interface IExamPaperFactory
{
    /// <summary>
    /// метод для создания билета
    /// </summary>
    /// <param name="id">id билета</param>
    /// <param name="name">имя билета</param>
    /// <param name="questions">списовк вопросов для создания билета </param>
    /// <returns></returns>
    IExamPaper CreateExamPaper(Guid id, string name, IEnumerable<IQuestion> questions);
}