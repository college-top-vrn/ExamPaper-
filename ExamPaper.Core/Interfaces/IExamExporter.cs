using System.Collections.Generic;

namespace ExamPaper.Core.Interfaces;

/// <summary>
/// Контракт для экспорта экзаменационных билетов в файл.
/// </summary>
public interface IExamExporter
{
    /// <summary>
    /// Метод для экспорта коллекции билетов в указанный файл.
    /// </summary>
    /// <param name="examPapers">Коллекция билетов для экспорта.</param>
    /// <param name="destinationPath">Путь к выходному файлу (PDF).</param>
    void Export(IEnumerable<IExamPaper> examPapers, string destinationPath);
}