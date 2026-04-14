using ExamPaper.Core.Interfaces;

namespace ExamPaper.Service.Factores;

public interface IQuestionFactory
{
    public IQuestion CreateQuestion(Guid id, string text);
}