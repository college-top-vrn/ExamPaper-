using System;
using System.Collections.Generic;
using System.Linq;

using ExamPaper.Core.Interfaces;

namespace ExamPaper.Core.Model;

public class QuestionModifiler : IQuestionModifier
{
    public List<IQuestion> Questions { get; } = new List<IQuestion>();
    
    public void AddQuestion(IQuestion question)
    {
        Questions.Add(question);
    }

    public void RemoveQuestion(Guid questionId)
    {
        IQuestion objToDelete = Questions.FirstOrDefault(x => x.Id == questionId);
        
        if (objToDelete != null)
        {
            Questions.Remove(objToDelete);
        }
    }

    public void SaveChanges()
    {
        throw new NotImplementedException();
    }
}