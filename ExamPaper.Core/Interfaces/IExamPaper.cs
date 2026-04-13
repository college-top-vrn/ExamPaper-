namespace ExamPaper.Core.Interfaces;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;



public interface IExamPaper
{
    Guid id { get; }
    string Title { get; }
    IReadOnlyCollection<IQuestion> Questions { get; }
}
