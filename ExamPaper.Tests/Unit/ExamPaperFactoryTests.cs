using System;
using System.Collections.Generic;
using System.Linq;

using ExamPaper.Core.Interfaces;
using ExamPaper.Core.Models;
using ExamPaper.Service.Factories;

using Xunit;

namespace ExamPaper.Tests.Unit;

/// <summary>
///     Содержит модульные тесты для проверки корректности работы фабрики <see cref="ExamPaperFactory" />.
/// </summary>
/// <remarks>
///     Данный тестовый набор проверяет как успешные сценарии создания экзаменационных листов,
///     так и корректность обработки исключительных ситуаций (валидацию входных данных).
/// </remarks>
public class ExamPaperFactoryTests
{
    private readonly ExamPaperFactory _factory;

    /// <summary>
    ///     Инициализирует новый экземпляр класса <see cref="ExamPaperFactoryTests" />.
    /// </summary>
    public ExamPaperFactoryTests()
    {
        _factory = new ExamPaperFactory();
    }

    #region Позитивные тесты

    /// <summary>
    ///     Проверяет, что фабрика создает валидный экземпляр <see cref="IExamPaper" /> при корректных входных данных.
    /// </summary>
    /// <remarks>
    ///     Проверяемые аспекты:
    ///     <list type="bullet">
    ///         <item>Объект не является null.</item>
    ///         <item>Объект реализует интерфейс IExamPaper.</item>
    ///         <item>Свойства Id и Title соответствуют переданным значениям.</item>
    ///         <item>Коллекция вопросов сохраняет целостность и порядок.</item>
    ///     </list>
    /// </remarks>
    [Fact]
    public void CreateExamPaper_WithValidData_ShouldReturnCorrectInstance()
    {
        // Arrange
        Guid expectedId = Guid.NewGuid();
        string expectedTitle = "Паттерны проектирования на C#";
        List<IQuestion> questions =
            [new Question("Что такое Singleton?"), new Question("Опишите паттерн Factory Method.")];

        IExamPaper result = _factory.CreateExamPaper(expectedId, expectedTitle, questions);

        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.IsAssignableFrom<IExamPaper>(result),
            () => Assert.Equal(expectedId, result.Id),
            () => Assert.Equal(expectedTitle, result.Title),
            () => Assert.Equal(questions.Count, result.Questions.Count()),
            () => Assert.Equal(questions[0].Text, result.Questions.First().Text)
        );
    }

    #endregion

    #region Негативные тесты

    /// <summary>
    ///     Проверяет, что метод выбрасывает <see cref="ArgumentException" /> (или производные),
    ///     если заголовок билета является пустым или равен null.
    /// </summary>
    /// <param name="invalidTitle">Некорректное значение заголовка, передаваемое из InlineData.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void CreateExamPaper_InvalidTitle_ShouldThrowArgumentException(string? invalidTitle)
    {
        Guid id = Guid.NewGuid();
        List<IQuestion> questions = new();

        Assert.ThrowsAny<ArgumentException>(() =>
            _factory.CreateExamPaper(id, invalidTitle, questions)
        );
    }

    /// <summary>
    ///     Проверяет генерацию <see cref="ArgumentNullException" /> при попытке передать null
    ///     вместо коллекции вопросов.
    /// </summary>
    [Fact]
    public void CreateExamPaper_NullQuestions_ShouldThrowArgumentNullException()
    {
        Guid id = Guid.NewGuid();
        string title = "Тестовый билет";

        Assert.Throws<ArgumentNullException>(() =>
            _factory.CreateExamPaper(id, title, null!));
    }

    #endregion
}