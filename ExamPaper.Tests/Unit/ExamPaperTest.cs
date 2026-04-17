using ExamPaper.Core.Interfaces;
using ExamPaper.Core.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace ExamPaper.Core.Tests.Models
{
    public class ExamPaperTests
    {
        private class MockQuestion : IQuestion
        {
            public Guid Id { get; init; }
            public string Text { get; init; }
        }

        [Fact]
        public void Constructor_ValidParameters_WorksCorrectly()
        {
            // Arrange
            var expectedId = Guid.NewGuid();
            var expectedTitle = "Тестовый билет";
            var questions = new List<IQuestion> { new MockQuestion() };

            // Act
            var examPaper = new Core.Models.ExamPaper(expectedId, expectedTitle, questions);

            // Assert
            Assert.Equal(expectedId, examPaper.Id);
            Assert.Equal(expectedTitle, examPaper.Title);
            Assert.Equal(1, examPaper.Questions.Count);
        }

        [Fact]
        public void Constructor_NullTitle_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new Core.Models.ExamPaper(Guid.NewGuid(), null, new List<IQuestion>()));
        }

        [Fact]
        public void Constructor_NullQuestions_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new Core.Models.ExamPaper(Guid.NewGuid(), "Title", null));
        }

        [Fact]
        public void Constructor_EmptyQuestionsList_CreatesEmptyReadOnlyCollection()
        {
            // Arrange
            var expectedId = Guid.NewGuid();
            var expectedTitle = "Пустой билет";

            // Act
            var examPaper = new Core.Models.ExamPaper(expectedId, expectedTitle, new List<IQuestion>());

            // Assert
            Assert.Equal(expectedId, examPaper.Id);
            Assert.Equal(expectedTitle, examPaper.Title);
            Assert.Equal(0, examPaper.Questions.Count);
            Assert.IsAssignableFrom<IReadOnlyCollection<IQuestion>>(examPaper.Questions);
        }
    }
}