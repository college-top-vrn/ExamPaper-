using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

using ExamPaper.Core.Interfaces;
using ExamPaper.Infrastructure.Exporter;

using Moq;

using Xunit;

namespace ExamPaper.Tests.Unit;

/// <summary>
///     Класс для Unit тестов JSON экспортера <see cref="JsonExamExporter" />.
/// </summary>
public class JsonExamExporterTests
{
    private static readonly JsonExamExporter Exporter = new();

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
            .Select(text =>
            {
                Mock<IQuestion> mockQuestion = new();
                mockQuestion.SetupGet(q => q.Id).Returns(Guid.CreateVersion7());
                mockQuestion.SetupGet(q => q.Text).Returns(text);
                return mockQuestion.Object;
            })
            .ToList();

        Mock<IExamPaper> mockPaper = new();
        mockPaper.SetupGet(p => p.Id).Returns(id);
        mockPaper.SetupGet(p => p.Title).Returns(title);
        mockPaper.SetupGet(p => p.Questions).Returns(questions.AsReadOnly());
        return mockPaper.Object;
    }

    /// <summary>
    ///     Позитивный параметризованный тест на удачный экспорт JSON с верным контентом из списка билетов.
    /// </summary>
    /// <param name="title">Название билета.</param>
    /// <param name="questionTexts">Текста вопросов билета.</param>
    [InlineData("Билет JSON", "Вопрос A", "Вопрос B")]
    [InlineData("Билет JSON2", "Вопрос A2", "Вопрос B2")]
    [Theory]
    public void Export_ValidExamPapers_ReturnsJsonWithCorrectContent(
        string title,
        params string[] questionTexts
    )
    {
        // Arrange
        Guid paperId = Guid.CreateVersion7();
        List<IExamPaper> papers = new() { CreateTestPaper(paperId, title, questionTexts) };

        // Act
        byte[] result = Exporter.Export(papers);
        using JsonDocument jsonDoc = JsonDocument.Parse(result);
        JsonElement root = jsonDoc.RootElement;
        JsonElement paperElement = root[0];
        JsonElement questionsElement = paperElement.GetProperty("questions");

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(paperId.ToString(), paperElement.GetProperty("id").GetString());
        Assert.Equal(title, paperElement.GetProperty("title").GetString());
        Assert.Equal(questionTexts.Length, questionsElement.GetArrayLength());
        int index = 0;
        foreach (JsonElement question in questionsElement.EnumerateArray())
        {
            Assert.Equal(questionTexts[index], question.GetProperty("text").GetString());
            index++;
        }
    }

    /// <summary>
    ///     Негативный тест на неудачный экспорт JSON из билета с неверным геттером.
    /// </summary>
    [Fact]
    public void Export_ExamPaperWithFaultyGetter_ThrowsException()
    {
        // Arrange
        Mock<IExamPaper> mockPaper = new();
        mockPaper.SetupGet(p => p.Id).Returns(Guid.NewGuid());
        mockPaper.SetupGet(p => p.Title).Throws(new InvalidOperationException("Test exception"));
        mockPaper.SetupGet(p => p.Questions).Returns(new List<IQuestion>().AsReadOnly());

        List<IExamPaper> papers = new() { mockPaper.Object };

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => Exporter.Export(papers));
    }
}