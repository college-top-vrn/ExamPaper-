using System;

namespace ExamPaper.Core.Interfaces;

/// <summary>
/// Репозиторий для управления вопросами. Объединяет чтение и модификацию данных.
/// </summary>
public interface IQuestionRepository : IQuestionProvider, IQuestionModifier
{
    /// <summary>
    /// Получает вопрос по его уникальному идентификатору.
    /// </summary>
    /// <param name="id">GUID вопроса.</param>
    /// <returns>Вопрос или null, если не найден.</returns>
    IQuestion? GetQuestionById(Guid id);
}