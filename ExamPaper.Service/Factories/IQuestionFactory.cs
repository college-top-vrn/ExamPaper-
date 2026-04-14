using ExamPaper.Core.Interfaces;

namespace ExamPaper.Service.Factores;

public interface IQuestionFactory : IQuestion
{
    public IQuestion CreateQuestion();
}