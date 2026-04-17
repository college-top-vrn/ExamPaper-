using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ExamPaper.Core.Interfaces;
using ExamPaper.Core.Models;
using ExamPaper.Infrastructure.Repositories;

using Xunit;

namespace ExamPaper.Tests.Unit;

public class QuestionRepositoryTests : IDisposable
{
    private readonly string _tempFilePath;

    public QuestionRepositoryTests()
    {
        _tempFilePath = Path.GetTempFileName();
    }

    [Fact]
    public void Constructor_WithValidPath_CreatesRepository()
    {
        var repository = new QuestionRepository(_tempFilePath);
        Assert.NotNull(repository);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidPath_ThrowsArgumentException(string invalidPath)
    {
        Assert.Throws<ArgumentException>(() => new QuestionRepository(invalidPath));
    }

    [Fact]
    public void Constructor_WithNullPath_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new QuestionRepository(null));
    }

    [Fact]
    public void AddAndGetQuestion_WorkCorrectly()
    {
        var repository = new QuestionRepository(_tempFilePath);
        var questionId = Guid.NewGuid();
        var question = new Question(questionId, "Test Question");

        repository.AddQuestion(question);
        var retrieved = repository.GetQuestionById(questionId);

        Assert.NotNull(retrieved);
        Assert.Equal(question.Text, retrieved!.Text);
        Assert.Equal(questionId, retrieved.Id);
    }

    [Fact]
    public void AddDuplicateQuestion_ThrowsException()
    {
        var repository = new QuestionRepository(_tempFilePath);
        var id = Guid.NewGuid();
        var question1 = new Question(id, "First");
        var question2 = new Question(id, "Second");

        repository.AddQuestion(question1);

        Assert.Throws<InvalidOperationException>(() => repository.AddQuestion(question2));
    }

    [Fact]
    public void RemoveExistingQuestion_RemovesSuccessfully()
    {
        var repository = new QuestionRepository(_tempFilePath);
        var questionId = Guid.NewGuid();
        var question = new Question(questionId, "To Remove");

        repository.AddQuestion(question);
        Assert.Single(repository.GetAllQuestions());

        repository.RemoveQuestion(questionId);
        Assert.Empty(repository.GetAllQuestions());
    }

    [Fact]
    public void RemoveNonExistentQuestion_DoesNothing()
    {
        var repository = new QuestionRepository(_tempFilePath);
        var nonExistentId = Guid.NewGuid();

        repository.RemoveQuestion(nonExistentId);
        Assert.Empty(repository.GetAllQuestions());
    }

    [Fact]
    public void SaveChanges_PersistsQuestionsToFile()
    {
        var repository = new QuestionRepository(_tempFilePath);
        var questionId = Guid.NewGuid();
        var question = new Question(questionId, "Persisted Question");

        repository.AddQuestion(question);
        repository.SaveChanges();

        var fileContent = File.ReadAllText(_tempFilePath);
        Assert.Contains(question.Text, fileContent);
        Assert.Contains(questionId.ToString(), fileContent);
    }

    [Fact]
    public void LoadFromFile_RestoresPreviousState()
    {
        // First repository - add data and save
        var repository1 = new QuestionRepository(_tempFilePath);
        var questionId = Guid.NewGuid();
        var question = new Question(questionId, "Persisted");
        repository1.AddQuestion(question);
        repository1.SaveChanges();

        // Second repository - load from same file
        var repository2 = new QuestionRepository(_tempFilePath);
        var loadedQuestions = repository2.GetAllQuestions().ToList();

        Assert.Single(loadedQuestions);
        Assert.Equal(questionId, loadedQuestions.First().Id);
        Assert.Equal(question.Text, loadedQuestions.First().Text);
    }

    [Fact]
    public void GetAllQuestions_ReturnsReadOnlyCollection()
    {
        var repository = new QuestionRepository(_tempFilePath);
        var questions = repository.GetAllQuestions();

        Assert.IsAssignableFrom<IReadOnlyList<IQuestion>>(questions);
    }

    [Fact]
    public void GetQuestionById_WithMultipleQuestions_ReturnsCorrect()
    {
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

        var found = repository.GetQuestionById(targetId);

        Assert.NotNull(found);
        Assert.Equal("Target", found!.Text);
    }

    [Fact]
    public void AddNullQuestion_ThrowsArgumentNullException()
    {
        var repository = new QuestionRepository(_tempFilePath);
        Assert.Throws<ArgumentNullException>(() => repository.AddQuestion(null));
    }

    [Fact]
    public void WhenFileExistsWithInvalidJson_LoadsEmptyCollection()
    {
        File.WriteAllText(_tempFilePath, "{ invalid json }");

        var repository = new QuestionRepository(_tempFilePath);
        var questions = repository.GetAllQuestions();

        Assert.Empty(questions);
    }

    [Fact]
    public void WhenFileExistsWithEmptyJson_LoadsEmptyCollection()
    {
        File.WriteAllText(_tempFilePath, "");

        var repository = new QuestionRepository(_tempFilePath);
        var questions = repository.GetAllQuestions();

        Assert.Empty(questions);
    }

    [Fact]
    public void WhenFileDoesNotExist_InitializesEmptyCollection()
    {
        var nonExistentFile = Path.GetTempFileName();
        File.Delete(nonExistentFile);

        var repository = new QuestionRepository(nonExistentFile);
        var questions = repository.GetAllQuestions();

        Assert.Empty(questions);

        // Cleanup
        if (File.Exists(nonExistentFile))
            File.Delete(nonExistentFile);
    }

    [Fact]
    public void AddMultipleQuestions_AllAddedSuccessfully()
    {
        var repository = new QuestionRepository(_tempFilePath);
        var questions = new[]
        {
            new Question(Guid.NewGuid(), "Question 1"),
            new Question(Guid.NewGuid(), "Question 2"),
            new Question(Guid.NewGuid(), "Question 3"),
        };

        foreach (var q in questions)
        {
            repository.AddQuestion(q);
        }

        var allQuestions = repository.GetAllQuestions();
        Assert.Equal(3, allQuestions.Count());
    }

    [Fact]
    public void GetQuestionById_WhenNotFound_ReturnsNull()
    {
        var repository = new QuestionRepository(_tempFilePath);
        var nonExistentId = Guid.NewGuid();

        var result = repository.GetQuestionById(nonExistentId);

        Assert.Null(result);
    }

    public void Dispose()
    {
        if (File.Exists(_tempFilePath))
            File.Delete(_tempFilePath);
    }
}