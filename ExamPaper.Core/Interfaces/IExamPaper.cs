namespace ExamPaper.Core.Interfaces;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;




public interface IExamPaper
{
    Guid id { get; }
    string Title { get; }
    /// <summary>Список вопросов, попавших в данный билет.</summary>
    IReadOnlyCollection<IQuestion> Questions { get; }
}
