using System;
using System.Collections.Generic;
using System.Linq;
using ExamPaper.Core.Generation;
using ExamPaper.Core.Interfaces;
namespace ExamPaper.Service.Generator;
public class ExamPaperGenerator : IExamGenerator
{
    public IEnumerable<IExamPaper> Generate(IEnumerable<IQuestion> availableQuestions, IGenerationSettings settings)
    {
        var questions = availableQuestions.ToList();
        var random = new Random();
        return Enumerable.Range(1, settings.TotalTicketsCount)
            .Select(ticketNum => new ExamPaper(
                ticketNum, 
                questions
                    .OrderBy(x => random.Next())
                    .Take(settings.QuestionsPerTicketCount)
                    .ToList()));
    }
}
