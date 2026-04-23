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
///     Набор тестов для проверки класса <see cref="QuestionRepository" />.
/// </summary>
/// <remarks>
///     <para>
///         Тесты покрывают следующие аспекты работы репозитория вопросов:
///     </para>
///     <list type="number">
///         <item>
///             <description>Конструкторы и валидация пути к файлу</description>
///         </item>
///         <item>
///             <description>CRUD операции (Add, Get, Remove)</description>
///         </item>
///         <item>
///             <description>Сохранение и загрузка данных из JSON-файла</description>
///         </item>
///         <item>
///             <description>Обработка ошибок и краевых случаев</description>
///         </item>
///         <item>
///             <description>Работа с коллекциями (множественное добавление, поиск)</description>
///         </item>
///     </list>
///     <para>
///         Каждый тест создаёт временный файл, который автоматически удаляется после выполнения теста.
///     </para>
/// </remarks>
public class QuestionRepositoryTests : IDisposable
{
    private readonly string _tempFilePath;

    /// <summary>
    ///     Инициализирует новый экземпляр тестового класса.
    ///     Создаёт временный файл для каждого теста.
    /// </summary>
    public QuestionRepositoryTests()
    {
        _tempFilePath = Path.GetTempFileName();
    }

    /// <summary>
    ///     Освобождает ресурсы, удаляя временный файл после выполнения теста.
    /// </summary>
    public void Dispose()
    {
        if (File.Exists(_tempFilePath))
        {
            File.Delete(_tempFilePath);
        }
    }

    /// <summary>
    ///     Проверяет, что конструктор с валидным путём успешно создаёт экземпляр репозитория.
    /// </summary>
    [Fact]
    public void Constructor_WithValidPath_CreatesRepository()
    {
        // Act
        QuestionRepository repository = new(_tempFilePath);

        // Assert
        Assert.NotNull(repository);
    }

    /// <summary>
    ///     Проверяет, что конструктор выбрасывает <see cref="ArgumentException" />
    ///     при передаче пустого пути к файлу.
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
    ///     Проверяет, что конструктор выбрасывает <see cref="ArgumentException" />
    ///     при передаче <c>null</c> в качестве пути к файлу.
    /// </summary>
    [Fact]
    public void Constructor_WithNullPath_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new QuestionRepository(null));
    }

    /// <summary>
    ///     Проверяет корректность добавления вопроса и получения его по идентификатору.
    /// </summary>
    /// <remarks>
    ///     Тест выполняет следующие шаги:
    ///     1. Создаёт вопрос с уникальным идентификатором
    ///     2. Добавляет его в репозиторий
    ///     3. Получает вопрос по идентификатору
    ///     4. Проверяет, что текст и идентификатор совпадают с исходными
    /// </remarks>
    [Fact]
    public void AddAndGetQuestion_WorkCorrectly()
    {
        // Arrange
        QuestionRepository repository = new(_tempFilePath);
        Guid questionId = Guid.NewGuid();
        Question question = new(questionId, "Test Question");

        // Act
        repository.AddQuestion(question);
        IQuestion? retrieved = repository.GetQuestionById(questionId);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(question.Text, retrieved!.Text);
        Assert.Equal(questionId, retrieved.Id);
    }

    /// <summary>
    ///     Проверяет, что при попытке добавить вопрос с уже существующим идентификатором
    ///     выбрасывается исключение <see cref="InvalidOperationException" />.
    /// </summary>
    [Fact]
    public void AddDuplicateQuestion_ThrowsException()
    {
        // Arrange
        QuestionRepository repository = new(_tempFilePath);
        Guid id = Guid.NewGuid();
        Question question1 = new(id, "First");
        Question question2 = new(id, "Second");

        repository.AddQuestion(question1);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => repository.AddQuestion(question2));
    }

    /// <summary>
    ///     Проверяет, что существующий вопрос успешно удаляется из репозитория.
    /// </summary>
    /// <remarks>
    ///     Тест проверяет, что после удаления вопроса коллекция становится пустой.
    /// </remarks>
    [Fact]
    public void RemoveExistingQuestion_RemovesSuccessfully()
    {
        // Arrange
        QuestionRepository repository = new(_tempFilePath);
        Guid questionId = Guid.NewGuid();
        Question question = new(questionId, "To Remove");

        repository.AddQuestion(question);
        Assert.Single(repository.GetAllQuestions());

        // Act
        repository.RemoveQuestion(questionId);

        // Assert
        Assert.Empty(repository.GetAllQuestions());
    }

    /// <summary>
    ///     Проверяет, что попытка удалить несуществующий вопрос не вызывает ошибку
    ///     и не изменяет состояние репозитория.
    /// </summary>
    [Fact]
    public void RemoveNonExistentQuestion_DoesNothing()
    {
        // Arrange
        QuestionRepository repository = new(_tempFilePath);
        Guid nonExistentId = Guid.NewGuid();

        // Act & Assert
        repository.RemoveQuestion(nonExistentId);
        Assert.Empty(repository.GetAllQuestions());
    }

    /// <summary>
    ///     Проверяет, что метод <see cref="QuestionRepository.SaveChanges" /> успешно сохраняет
    ///     вопросы в JSON-файл.
    /// </summary>
    /// <remarks>
    ///     Тест проверяет, что после сохранения файл содержит текст вопроса и его идентификатор.
    /// </remarks>
    [Fact]
    public void SaveChanges_PersistsQuestionsToFile()
    {
        // Arrange
        QuestionRepository repository = new(_tempFilePath);
        Guid questionId = Guid.NewGuid();
        Question question = new(questionId, "Persisted Question");

        // Act
        repository.AddQuestion(question);
        repository.SaveChanges();

        // Assert
        string fileContent = File.ReadAllText(_tempFilePath);
        Assert.Contains(question.Text, fileContent);
        Assert.Contains(questionId.ToString(), fileContent);
    }

    /// <summary>
    ///     Проверяет, что вопросы корректно загружаются из JSON-файла при создании нового экземпляра репозитория.
    /// </summary>
    /// <remarks>
    ///     Тест выполняет следующие шаги:
    ///     1. Создаёт репозиторий, добавляет вопрос и сохраняет в файл
    ///     2. Создаёт новый репозиторий (который автоматически загружает данные из файла)
    ///     3. Проверяет, что загруженные данные соответствуют сохранённым
    /// </remarks>
    [Fact]
    public void LoadFromFile_RestoresPreviousState()
    {
        // Arrange - First repository
        QuestionRepository repository1 = new(_tempFilePath);
        Guid questionId = Guid.NewGuid();
        Question question = new(questionId, "Persisted");
        repository1.AddQuestion(question);
        repository1.SaveChanges();

        // Act - Second repository loads from file
        QuestionRepository repository2 = new(_tempFilePath);
        List<IQuestion> loadedQuestions = repository2.GetAllQuestions().ToList();

        // Assert
        Assert.Single(loadedQuestions);
        Assert.Equal(questionId, loadedQuestions.First().Id);
        Assert.Equal(question.Text, loadedQuestions.First().Text);
    }

    /// <summary>
    ///     Проверяет, что метод <see cref="QuestionRepository.GetAllQuestions" />
    ///     возвращает коллекцию, доступную только для чтения.
    /// </summary>
    [Fact]
    public void GetAllQuestions_ReturnsReadOnlyCollection()
    {
        // Arrange
        QuestionRepository repository = new(_tempFilePath);

        // Act
        IEnumerable<IQuestion> questions = repository.GetAllQuestions();

        // Assert
        Assert.IsAssignableFrom<IReadOnlyList<IQuestion>>(questions);
    }

    /// <summary>
    ///     Проверяет, что метод <see cref="QuestionRepository.GetQuestionById" />
    ///     возвращает правильный вопрос при наличии нескольких вопросов в репозитории.
    /// </summary>
    [Fact]
    public void GetQuestionById_WithMultipleQuestions_ReturnsCorrect()
    {
        // Arrange
        QuestionRepository repository = new(_tempFilePath);
        Guid targetId = Guid.NewGuid();

        List<Question> questions = new()
        {
            new Question(Guid.NewGuid(), "Q1"), new Question(targetId, "Target"), new Question(Guid.NewGuid(), "Q3")
        };

        foreach (Question q in questions)
        {
            repository.AddQuestion(q);
        }

        // Act
        IQuestion? found = repository.GetQuestionById(targetId);

        // Assert
        Assert.NotNull(found);
        Assert.Equal("Target", found!.Text);
    }

    /// <summary>
    ///     Проверяет, что метод <see cref="QuestionRepository.AddQuestion" />
    ///     выбрасывает исключение <see cref="ArgumentNullException" /> при попытке добавить <c>null</c>.
    /// </summary>
    [Fact]
    public void AddNullQuestion_ThrowsArgumentNullException()
    {
        // Arrange
        QuestionRepository repository = new(_tempFilePath);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => repository.AddQuestion(null));
    }

    /// <summary>
    ///     Проверяет, что при загрузке из файла с невалидным JSON содержимым
    ///     репозиторий инициализируется с пустой коллекцией.
    /// </summary>
    [Fact]
    public void WhenFileExistsWithInvalidJson_LoadsEmptyCollection()
    {
        // Arrange
        File.WriteAllText(_tempFilePath, "{ invalid json }");

        // Act
        QuestionRepository repository = new(_tempFilePath);
        IEnumerable<IQuestion> questions = repository.GetAllQuestions();

        // Assert
        Assert.Empty(questions);
    }

    /// <summary>
    ///     Проверяет, что при загрузке из пустого JSON-файла
    ///     репозиторий инициализируется с пустой коллекцией.
    /// </summary>
    [Fact]
    public void WhenFileExistsWithEmptyJson_LoadsEmptyCollection()
    {
        // Arrange
        File.WriteAllText(_tempFilePath, "");

        // Act
        QuestionRepository repository = new(_tempFilePath);
        IEnumerable<IQuestion> questions = repository.GetAllQuestions();

        // Assert
        Assert.Empty(questions);
    }

    /// <summary>
    ///     Проверяет, что при создании репозитория с путём к несуществующему файлу
    ///     инициализируется пустая коллекция.
    /// </summary>
    [Fact]
    public void WhenFileDoesNotExist_InitializesEmptyCollection()
    {
        // Arrange
        string nonExistentFile = Path.GetTempFileName();
        File.Delete(nonExistentFile);

        // Act
        QuestionRepository repository = new(nonExistentFile);
        IEnumerable<IQuestion> questions = repository.GetAllQuestions();

        // Assert
        Assert.Empty(questions);

        // Cleanup
        if (File.Exists(nonExistentFile))
        {
            File.Delete(nonExistentFile);
        }
    }

    /// <summary>
    ///     Проверяет, что метод <see cref="QuestionRepository.AddQuestion" />
    ///     корректно добавляет несколько вопросов в репозиторий.
    /// </summary>
    [Fact]
    public void AddMultipleQuestions_AllAddedSuccessfully()
    {
        // Arrange
        QuestionRepository repository = new(_tempFilePath);
        Question[] questions = new[]
        {
            new Question(Guid.NewGuid(), "Question 1"), new Question(Guid.NewGuid(), "Question 2"),
            new Question(Guid.NewGuid(), "Question 3")
        };

        // Act
        foreach (Question q in questions)
        {
            repository.AddQuestion(q);
        }

        // Assert
        IEnumerable<IQuestion> allQuestions = repository.GetAllQuestions();
        Assert.Equal(3, allQuestions.Count());
    }

    /// <summary>
    ///     Проверяет, что метод <see cref="QuestionRepository.GetQuestionById" />
    ///     возвращает <c>null</c> при поиске несуществующего вопроса.
    /// </summary>
    [Fact]
    public void GetQuestionById_WhenNotFound_ReturnsNull()
    {
        // Arrange
        QuestionRepository repository = new(_tempFilePath);
        Guid nonExistentId = Guid.NewGuid();

        // Act
        IQuestion? result = repository.GetQuestionById(nonExistentId);

        // Assert
        Assert.Null(result);
    }
}