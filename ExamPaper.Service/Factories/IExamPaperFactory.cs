using ExamPaper.Core.Interfaces;

namespace ExamPaper.Service.Factores;

public interface IExamPaperFactory : IExamPaper
{
    public IExamPaper CreateExamPaper(string name, IEnumerable<IQuestion> questions);
}