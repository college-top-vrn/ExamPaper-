using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ExamPaper.Core.Interfaces;
using ExamPaper.Core.Models;
using ExamPaper.Infrastructure.Repositories;

using Xunit;

namespace ExamPaper.Tests.Unit;

/// <summary>
/// Набор тестов для проверки класса <see cref="QuestionRepository"/>.
/// </summary>
/// <remarks>
/// <para>
/// Тесты покрывают следующие аспекты работы репозитория вопросов:
/// </para>
/// <list type="number">
/// <item><description>Конструкторы и валидация пути к файлу</description></item>
/// <item><description>CRUD операции (Add, Get, Remove)</description></item>
/// <item><description>Сохранение и загрузка данных из JSON-файла</description></item>
/// <item><description>Обработка ошибок и краевых случаев</description></item>
/// <item><description>Работа с коллекциями (множественное добавление, поиск)</description></item>
/// </list>
/// <para>
/// Каждый тест создаёт временный файл, который автоматически удаляется после выполнения теста.
/// </para>
/// </remarks>
public class QuestionRepositoryTests : IDisposable
{
    private readonly string _tempFilePath;

    /// <summary>
    /// Инициализирует новый экземпляр тестового класса.
    /// Создаёт временный файл для каждого теста.
    /// </summary>
    public QuestionRepositoryTests()
    {
        _tempFilePath = Path.GetTempFileName();
    }

    /// <summary>
    /// Проверяет, что конструктор с валидным путём успешно создаёт экземпляр репозитория.
    /// </summary>
    [Fact]
    public void Constructor_WithValidPath_CreatesRepository()
    {
        // Act
        var repository = new QuestionRepository(_tempFilePath);

        // Assert
        Assert.NotNull(repository);
    }

    /// <summary>
    /// Проверяет, что конструктор выбрасывает <see cref="ArgumentException"/>
    /// при передаче пустого пути к файлу.
    /// </summary>
    /// <param name="invalidPath">Пустой путь к файлу (пустая строка или пробелы).</param>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidPath_ThrowsArgumentException(string invalidPath)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new QuestionRepository(invalidPath));
    }

    /// <summary>
    /// Проверяет, что конструктор выбрасывает <see cref="ArgumentException"/>
    /// при передаче <c>null</c> в качестве пути к файлу.
    /// </summary>
    [Fact]
    public void Constructor_WithNullPath_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new QuestionRepository(null));
    }

    /// <summary>
    /// Проверяет корректность добавления вопроса и получения его по идентификатору.
    /// </summary>
    /// <remarks>
    /// Тест выполняет следующие шаги:
    /// 1. Создаёт вопрос с уникальным идентификатором
    /// 2. Добавляет его в репозиторий
    /// 3. Получает вопрос по идентификатору
    /// 4. Проверяет, что текст и идентификатор совпадают с исходными
    /// </remarks>
    [Fact]
    public void AddAndGetQuestion_WorkCorrectly()
    {
        // Arrange
        var repository = new QuestionRepository(_tempFilePath);
        var questionId = Guid.NewGuid();
        var question = new Question(questionId, "Test Question");

        // Act
        repository.AddQuestion(question);
        var retrieved = repository.GetQuestionById(questionId);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(question.Text, retrieved!.Text);
        Assert.Equal(questionId, retrieved.Id);
    }

    /// <summary>
    /// Проверяет, что при попытке добавить вопрос с уже существующим идентификатором
    /// выбрасывается исключение <see cref="InvalidOperationException"/>.
    /// </summary>
    [Fact]
    public void AddDuplicateQuestion_ThrowsException()
    {
        // Arrange
        var repository = new QuestionRepository(_tempFilePath);
        var id = Guid.NewGuid();
        var question1 = new Question(id, "First");
        var question2 = new Question(id, "Second");

        repository.AddQuestion(question1);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => repository.AddQuestion(question2));
    }

    /// <summary>
    /// Проверяет, что существующий вопрос успешно удаляется из репозитория.
    /// </summary>
    /// <remarks>
    /// Тест проверяет, что после удаления вопроса коллекция становится пустой.
    /// </remarks>
    [Fact]
    public void RemoveExistingQuestion_RemovesSuccessfully()
    {
        // Arrange
        var repository = new QuestionRepository(_tempFilePath);
        var questionId = Guid.NewGuid();
        var question = new Question(questionId, "To Remove");

        repository.AddQuestion(question);
        Assert.Single(repository.GetAllQuestions());

        // Act
        repository.RemoveQuestion(questionId);

        // Assert
        Assert.Empty(repository.GetAllQuestions());
    }

    /// <summary>
    /// Проверяет, что попытка удалить несуществующий вопрос не вызывает ошибку
    /// и не изменяет состояние репозитория.
    /// </summary>
    [Fact]
    public void RemoveNonExistentQuestion_DoesNothing()
    {
        // Arrange
        var repository = new QuestionRepository(_tempFilePath);
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
        repository.RemoveQuestion(nonExistentId);
        Assert.Empty(repository.GetAllQuestions());
    }

    /// <summary>
    /// Проверяет, что метод <see cref="QuestionRepository.SaveChanges"/> успешно сохраняет
    /// вопросы в JSON-файл.
    /// </summary>
    /// <remarks>
    /// Тест проверяет, что после сохранения файл содержит текст вопроса и его идентификатор.
    /// </remarks>
    [Fact]
    public void SaveChanges_PersistsQuestionsToFile()
    {
        // Arrange
        var repository = new QuestionRepository(_tempFilePath);
        var questionId = Guid.NewGuid();
        var question = new Question(questionId, "Persisted Question");

        // Act
        repository.AddQuestion(question);
        repository.SaveChanges();

        // Assert
        var fileContent = File.ReadAllText(_tempFilePath);
        Assert.Contains(question.Text, fileContent);
        Assert.Contains(questionId.ToString(), fileContent);
    }

    /// <summary>
    /// Проверяет, что вопросы корректно загружаются из JSON-файла при создании нового экземпляра репозитория.
    /// </summary>
    /// <remarks>
    /// Тест выполняет следующие шаги:
    /// 1. Создаёт репозиторий, добавляет вопрос и сохраняет в файл
    /// 2. Создаёт новый репозиторий (который автоматически загружает данные из файла)
    /// 3. Проверяет, что загруженные данные соответствуют сохранённым
    /// </remarks>
    [Fact]
    public void LoadFromFile_RestoresPreviousState()
    {
        // Arrange - First repository
        var repository1 = new QuestionRepository(_tempFilePath);
        var questionId = Guid.NewGuid();
        var question = new Question(questionId, "Persisted");
        repository1.AddQuestion(question);
        repository1.SaveChanges();

        // Act - Second repository loads from file
        var repository2 = new QuestionRepository(_tempFilePath);
        var loadedQuestions = repository2.GetAllQuestions().ToList();

        // Assert
        Assert.Single(loadedQuestions);
        Assert.Equal(questionId, loadedQuestions.First().Id);
        Assert.Equal(question.Text, loadedQuestions.First().Text);
    }

    /// <summary>
    /// Проверяет, что метод <see cref="QuestionRepository.GetAllQuestions"/>
    /// возвращает коллекцию, доступную только для чтения.
    /// </summary>
    [Fact]
    public void GetAllQuestions_ReturnsReadOnlyCollection()
    {
        // Arrange
        var repository = new QuestionRepository(_tempFilePath);

        // Act
        var questions = repository.GetAllQuestions();

        // Assert
        Assert.IsAssignableFrom<IReadOnlyList<IQuestion>>(questions);
    }

    /// <summary>
    /// Проверяет, что метод <see cref="QuestionRepository.GetQuestionById"/>
    /// возвращает правильный вопрос при наличии нескольких вопросов в репозитории.
    /// </summary>
    [Fact]
    public void GetQuestionById_WithMultipleQuestions_ReturnsCorrect()
    {
        // Arrange
        var repository = new QuestionRepository(_tempFilePath);
        var targetId = Guid.NewGuid();

        var questions = new List<Question>
        {
            new Question(Guid.NewGuid(), "Q1"),
            new Question(targetId, "Target"),
            new Question(Guid.NewGuid(), "Q3"),
        };

        foreach (var q in questions)
        {
            repository.AddQuestion(q);
        }

        // Act
        var found = repository.GetQuestionById(targetId);

        // Assert
        Assert.NotNull(found);
        Assert.Equal("Target", found!.Text);
    }

    /// <summary>
    /// Проверяет, что метод <see cref="QuestionRepository.AddQuestion"/>
    /// выбрасывает исключение <see cref="ArgumentNullException"/> при попытке добавить <c>null</c>.
    /// </summary>
    [Fact]
    public void AddNullQuestion_ThrowsArgumentNullException()
    {
        // Arrange
        var repository = new QuestionRepository(_tempFilePath);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => repository.AddQuestion(null));
    }

    /// <summary>
    /// Проверяет, что при загрузке из файла с невалидным JSON содержимым
    /// репозиторий инициализируется с пустой коллекцией.
    /// </summary>
    [Fact]
    public void WhenFileExistsWithInvalidJson_LoadsEmptyCollection()
    {
        // Arrange
        File.WriteAllText(_tempFilePath, "{ invalid json }");

        // Act
        var repository = new QuestionRepository(_tempFilePath);
        var questions = repository.GetAllQuestions();

        // Assert
        Assert.Empty(questions);
    }

    /// <summary>
    /// Проверяет, что при загрузке из пустого JSON-файла
    /// репозиторий инициализируется с пустой коллекцией.
    /// </summary>
    [Fact]
    public void WhenFileExistsWithEmptyJson_LoadsEmptyCollection()
    {
        // Arrange
        File.WriteAllText(_tempFilePath, "");

        // Act
        var repository = new QuestionRepository(_tempFilePath);
        var questions = repository.GetAllQuestions();

        // Assert
        Assert.Empty(questions);
    }

    /// <summary>
    /// Проверяет, что при создании репозитория с путём к несуществующему файлу
    /// инициализируется пустая коллекция.
    /// </summary>
    [Fact]
    public void WhenFileDoesNotExist_InitializesEmptyCollection()
    {
        // Arrange
        var nonExistentFile = Path.GetTempFileName();
        File.Delete(nonExistentFile);

        // Act
        var repository = new QuestionRepository(nonExistentFile);
        var questions = repository.GetAllQuestions();

        // Assert
        Assert.Empty(questions);

        // Cleanup
        if (File.Exists(nonExistentFile))
            File.Delete(nonExistentFile);
    }

    /// <summary>
    /// Проверяет, что метод <see cref="QuestionRepository.AddQuestion"/>
    /// корректно добавляет несколько вопросов в репозиторий.
    /// </summary>
    [Fact]
    public void AddMultipleQuestions_AllAddedSuccessfully()
    {
        // Arrange
        var repository = new QuestionRepository(_tempFilePath);
        var questions = new[]
        {
            new Question(Guid.NewGuid(), "Question 1"),
            new Question(Guid.NewGuid(), "Question 2"),
            new Question(Guid.NewGuid(), "Question 3"),
        };

        // Act
        foreach (var q in questions)
        {
            repository.AddQuestion(q);
        }

        // Assert
        var allQuestions = repository.GetAllQuestions();
        Assert.Equal(3, allQuestions.Count());
    }

    /// <summary>
    /// Проверяет, что метод <see cref="QuestionRepository.GetQuestionById"/>
    /// возвращает <c>null</c> при поиске несуществующего вопроса.
    /// </summary>
    [Fact]
    public void GetQuestionById_WhenNotFound_ReturnsNull()
    {
        // Arrange
        var repository = new QuestionRepository(_tempFilePath);
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = repository.GetQuestionById(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Освобождает ресурсы, удаляя временный файл после выполнения теста.
    /// </summary>
    public void Dispose()
    {
        if (File.Exists(_tempFilePath))
            File.Delete(_tempFilePath);
    }
}