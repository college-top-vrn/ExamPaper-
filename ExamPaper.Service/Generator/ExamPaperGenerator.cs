using System;
using System.Collections.Generic;
using System.Linq;

using ExamPaper.Core.Interfaces;

namespace ExamPaper.Service.Generator;

/// <summary>
///     Генератор экзаменационных билетов, создающий набор билетов со случайными вопросами.
/// </summary>
public class ExamPaperGenerator : IExamGenerator
{
    /// <summary>
    ///     Генерирует коллекцию экзаменационных билетов на основе доступных вопросов и настроек генерации.
    /// </summary>
    /// <param name="availableQuestions">Коллекция доступных вопросов для составления билетов.</param>
    /// <param name="settings">Настройки генерации, определяющие количество билетов и вопросов в каждом билете.</param>
    /// <returns>Коллекция сгенерированных экзаменационных билетов.</returns>
    /// <remarks>
    ///     Вопросы для каждого билета выбираются случайным образом без повторений в пределах одного билета.
    ///     Один и тот же вопрос может встречаться в разных билетах.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Выбрасывается, если <paramref name="availableQuestions" /> или
    ///     <paramref name="settings" /> равны null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Выбрасывается, если количество доступных вопросов меньше, чем
    ///     <see cref="IGenerationSettings.QuestionsPerTicketCount" />.
    /// </exception>
    public IEnumerable<IExamPaper> Generate(
        IEnumerable<IQuestion> availableQuestions,
        IGenerationSettings settings
    )
    {
        if (availableQuestions == null)
        {
            throw new ArgumentNullException(nameof(availableQuestions));
        }

        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        List<IQuestion> questions = availableQuestions.ToList();
        if (questions.Count < settings.QuestionsPerTicketCount)
        {
            throw new InvalidOperationException(
                $"Недостаточно вопросов для генерации билета. "
                + $"Доступно: {questions.Count}, требуется: {settings.QuestionsPerTicketCount}"
            );
        }

        Random random = new();
        return Enumerable
            .Range(1, settings.TotalTicketsCount)
            .Select(ticketNum => new Core.Models.ExamPaper(
                Guid.NewGuid(),
                $"Билет №{ticketNum}",
                questions
                    .OrderBy(_ => random.Next())
                    .Take(settings.QuestionsPerTicketCount)
                    .ToList()
            ));
    }
}