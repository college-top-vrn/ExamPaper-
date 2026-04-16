using System;
using System.Collections.Generic;

using ExamPaper.Core.Interfaces;

namespace ExamPaper.Service.Factories;

public class ExamPaperFactory : IExamPaperFactory
{
    public IExamPaper CreateExamPaper(Guid id, string name, IEnumerable<IQuestion> questions) => new ExamPaper(id,name, questions);

}