using System;
using System.Collections.Generic;
using System.Linq;

using ExamPaper.Core.Interfaces;

namespace ExamPaper.Core.Model;

/// <summary>
/// Позволяет управлять списком вопросов: добавлять, удалять и фиксировать изменения.
/// </summary>
public class QuestionModifiler : IQuestionModifier
{
    /// <summary>
    /// Список вопросов, с которым работает модификатор.
    /// </summary>
    public List<IQuestion> Questions { get; } = new List<IQuestion>();
    
    /// <summary>
    /// Добавляет новый вопрос в список.
    /// </summary>
    /// <param name="question">Вопрос, который нужно добавить.</param>
    public void AddQuestion(IQuestion question)
    {
        Questions.Add(question);
    }

    /// <summary>
    /// Удаляет вопрос из списка по указанному идентификатору.
    /// </summary>
    /// <param name="questionId">Идентификатор вопроса, который нужно удалить.</param>
    public void RemoveQuestion(Guid questionId)
    {
        IQuestion? objToDelete = Questions.FirstOrDefault(x => x.Id == questionId);
        
        if (objToDelete is not null)
        {
            Questions.Remove(objToDelete);
        }
    }

    /// <summary>
    /// Сохраняет изменения в вопросах (например, в базе данных или хранилище).
    /// Пока не реализовано.
    /// </summary>
    public void SaveChanges()
    {
        throw new NotImplementedException();
    }
}