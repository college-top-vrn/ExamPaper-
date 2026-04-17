using System.Collections.Generic;

using ExamPaper.Core.Generation;

namespace ExamPaper.Core.Interfaces;

/// <summary>
///     Определяет контракт для службы автоматизированной генерации экзаменационных билетов.
/// </summary>
/// <remarks>
///     Реализации данного интерфейса должны инкапсулировать алгоритмы выборки и перемешивания вопросов
///     без прямой зависимости от способов хранения данных или механизмов экспорта.
/// </remarks>
public interface IExamGenerator
{
    /// <summary>
    ///     Формирует коллекцию экзаменационных билетов на основе предоставленного пула вопросов и заданных правил.
    /// </summary>
    /// <param name="availableQuestions">Пул доступных вопросов, из которых будет производиться выборка.</param>
    /// <param name="settings">Объект конфигурации, определяющий параметры генерации (количество билетов, вопросов и т.д.).</param>
    /// <returns>Коллекция объектов, реализующих <see cref="IExamPaper" />, готовых к дальнейшей обработке или экспорту.</returns>
    /// <exception cref="System.ArgumentNullException">
    ///     Выбрасывается, если <paramref name="availableQuestions" /> или
    ///     <paramref name="settings" /> имеют значение null.
    /// </exception>
    /// <exception cref="System.InvalidOperationException">
    ///     Выбрасывается, если количество доступных вопросов недостаточно для
    ///     выполнения условий генерации.
    /// </exception>
    IEnumerable<IExamPaper> Generate(
        IEnumerable<IQuestion> availableQuestions,
        IGenerationSettings settings
    );
}