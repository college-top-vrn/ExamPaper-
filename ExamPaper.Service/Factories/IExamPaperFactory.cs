using System;
using System.Collections.Generic;

using ExamPaper.Core.Interfaces;

namespace ExamPaper.Service.Factories;

public interface IExamPaperFactory
{
     IExamPaper CreateExamPaper(Guid id,string name, IEnumerable<IQuestion> questions);
}