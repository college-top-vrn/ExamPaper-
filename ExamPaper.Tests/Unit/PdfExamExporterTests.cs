using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ExamPaper.Core.Interfaces;
using ExamPaper.Infrastructure.Exporter;

using Moq;

using UglyToad.PdfPig;

using Xunit;

namespace ExamPaper.Tests.Unit;

/// <summary>
///     Класс для Unit тестов PDF экспортера <see cref="PdfExamExporter" />.
/// </summary>
public class PdfExamExporterTests
{
    private static readonly PdfExamExporter Exporter = new();

    /// <summary>
    ///     Вспомогательный метод для создания билета.
    /// </summary>
    /// <param name="id">Id билета.</param>
    /// <param name="title">Название билета.</param>
    /// <param name="questionTexts">Текста вопросов билета.</param>
    /// <returns>Объект с интерфейсом билета.</returns>
    private static IExamPaper CreateTestPaper(Guid id, string title, params string[] questionTexts)
    {
        List<IQuestion> questions = questionTexts
            .Select((text, index) =>
                {
                    Mock<IQuestion> mockQuestion = new();
                    mockQuestion.SetupGet(q => q.Id).Returns(Guid.NewGuid());
                    mockQuestion.SetupGet(q => q.Text).Returns(text);
                    return mockQuestion.Object;
                }
            )
            .ToList();

        Mock<IExamPaper> mockPaper = new();
        mockPaper.SetupGet(p => p.Id).Returns(id);
        mockPaper.SetupGet(p => p.Title).Returns(title);
        mockPaper.SetupGet(p => p.Questions).Returns(questions.AsReadOnly());
        return mockPaper.Object;
    }

    /// <summary>
    ///     Позитивный параметризованный тест на удачный экспорт PDF с верным контентом из списка билетов.
    /// </summary>
    /// <param name="title">Название билета.</param>
    /// <param name="questionTexts">Текста вопросов билета.</param>
    [InlineData("Билет 1", "Вопрос 1.1", "Вопрос 1.2")]
    [InlineData("Билет 2", "Вопрос 2.1", "Вопрос 2.2", "Вопрос 2.3")]
    [Theory]
    public void Export_ValidExamPapers_ReturnsPdfWithCorrectContent(
        string title,
        params string[] questionTexts
    )
    {
        // Arrange
        Guid paperId = Guid.CreateVersion7();
        List<IExamPaper> papers = new() { CreateTestPaper(paperId, title, questionTexts) };

        // Act
        byte[] result = Exporter.Export(papers);
        string pdf = Encoding.ASCII.GetString(result);
        char[] header = pdf.Take(5).ToArray();

        // Assert
        using (PdfDocument pdfDocument = PdfDocument.Open(result))
        {
            // Собираем текст со всех страниц
            string allText = string.Join(" ", pdfDocument.GetPages().Select(p => p.Text));

            Assert.Contains(title, allText);
            Assert.Contains(questionTexts[0], allText);
            Assert.Contains(questionTexts[1], allText);
        }
    }

    /// <summary>
    ///     Негативный тест на неудачный экспорт PDF из пустого списка билетов.
    /// </summary>
    [Fact]
    public void Export_EmptyExamPapers_ThrowsException()
    {
        // Arrange
        List<IExamPaper> emptyPapers = new();

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => Exporter.Export(emptyPapers));
    }
}