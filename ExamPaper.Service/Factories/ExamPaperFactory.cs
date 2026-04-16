using System;
using System.Collections.Generic;

using ExamPaper.Core.Interfaces;

namespace ExamPaper.Service.Factories;
/// <summary>
/// класс фабрики ExamPaperFactory
/// </summary>
public class ExamPaperFactory : IExamPaperFactory
{
    /// <summary>
    /// метод фабрики для создания объекта ExamPaper где создается новй объекст ExamPaper
    /// </summary>
    /// <param name="id">id билета</param>
    /// <param name="name">имя билета</param>
    /// <param name="questions">список вопросов из которого берутся вопросы для сздания билета</param>
    /// <returns></returns>
    public IExamPaper CreateExamPaper(Guid id, string name, IEnumerable<IQuestion> questions) => new ExamPaper(id,name, questions);//TODO исправть ошибку с наименванием 

}