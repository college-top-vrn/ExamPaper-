using ExamPaper.Core.Interfaces;

namespace ExamPaper.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class ExamPaper : IExamPaper
{
    public Guid id { get; }
    public string Title { get; }
    public IReadOnlyCollection<IQuestion> Questions { get; }

    public ExamPaper(Guid id, string title, IEnumerable<IQuestion> questions)
    {
        this.id = id;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Questions = new ReadOnlyCollection<IQuestion>(
            (questions ?? throw new ArgumentNullException(nameof(questions))).ToList()
        );
    }
}