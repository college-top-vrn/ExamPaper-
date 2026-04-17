using System;

using ExamPaper.Core.Interfaces;
using ExamPaper.Core.Models;
using ExamPaper.Service.Factories;

using Xunit;

namespace ExamPaper.Tests.Unit;

/// <summary>
///     Тесты для фабрики <see cref="QuestionFactory" />.
///     Проверяют корректность создания объектов <see cref="Question" />.
/// </summary>
public class QuestionFactoryTests
{
    private readonly QuestionFactory _factory;

    /// <summary>
    ///     Конструктор теста, инициализирует фабрику
    /// </summary>
    public QuestionFactoryTests()
    {
        _factory = new QuestionFactory();
    }

    #region Негативные тесты

    /// <summary>
    ///     Проверяет, что фабрика выбрасывает исключение, если текст вопроса некорректен.
    /// </summary>
    /// <param name="invalidText">Невалидный текст (null, пустой, пробелы).</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateQuestion_InvalidText_ShouldThrowArgumentException(string? invalidText)
    {
        Guid id = Guid.CreateVersion7();

        Assert.Throws<ArgumentException>(() => _factory.CreateQuestion(id, invalidText!));
    }

    #endregion

    #region Позитивные тесты

    /// <summary>
    ///     Проверяет, что фабрика корректно передает данные в конструктор объекта.
    /// </summary>
    [Fact]
    public void CreateQuestion_ShouldReturnCorrectData()
    {
        Guid id = Guid.NewGuid();
        string text = "  Как работает Garbage Collector?  ";
        string expectedText = "Как работает Garbage Collector?"; // Ожидаем Trim

        IQuestion result = _factory.CreateQuestion(id, text);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(expectedText, result.Text); // Проверка, что Trim отработал
    }

    /// <summary>
    ///     Проверяет, что если передан Guid.Empty, фабрика вернет объект с сгенерированным Guid V7.
    /// </summary>
    [Fact]
    public void CreateQuestion_EmptyGuid_ShouldGenerateVersion7Guid()
    {
        Guid emptyId = Guid.Empty;
        string text = "Тестовый вопрос";

        IQuestion result = _factory.CreateQuestion(emptyId, text);

        Assert.NotEqual(Guid.Empty, result.Id);
    }

    #endregion
}