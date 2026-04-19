// using System;
// using System.Collections.Generic;
// using System.Linq;
//
// using ExamPaper.Core.Interfaces;
// using ExamPaper.Core.Models;
// using ExamPaper.Service.Generator;
//
// using Xunit;
//
// namespace ExamPaper.Tests.Unit;
//
// /// <summary>
// ///     Набор тестов для проверки класса <see cref="ExamPaperGenerator" />.
// /// </summary>
// /// <remarks>
// ///     <para>
// ///         Тесты покрывают следующие аспекты генерации экзаменационных билетов:
// ///     </para>
// ///     <list type="number">
// ///         <item>
// ///             <description>Валидация входных параметров (null проверки)</description>
// ///         </item>
// ///         <item>
// ///             <description>Проверка достаточности количества вопросов</description>
// ///         </item>
// ///         <item>
// ///             <description>Корректность количества генерируемых билетов</description>
// ///         </item>
// ///         <item>
// ///             <description>Корректность количества вопросов в каждом билете</description>
// ///         </item>
// ///         <item>
// ///             <description>Отсутствие повторений вопросов внутри одного билета</description>
// ///         </item>
// ///         <item>
// ///             <description>Возможность повторения вопросов в разных билетах</description>
// ///         </item>
// ///         <item>
// ///             <description>Случайность выборки вопросов</description>
// ///         </item>
// ///     </list>
// /// </remarks>
// public class ExamPaperGeneratorTests
// {
//     private readonly ExamPaperGenerator _generator;
//
//     /// <summary>
//     ///     Инициализирует новый экземпляр тестового класса.
//     /// </summary>
//     public ExamPaperGeneratorTests()
//     {
//         _generator = new ExamPaperGenerator();
//     }
//
//     /// <summary>
//     ///     Создаёт тестовый вопрос.
//     /// </summary>
//     private static IQuestion CreateTestQuestion(string text = "Test Question")
//     {
//         return new TestQuestion { Id = Guid.NewGuid(), Text = text };
//     }
//
//     /// <summary>
//     ///     Создаёт указанное количество тестовых вопросов.
//     /// </summary>
//     private static List<IQuestion> CreateTestQuestions(int count)
//     {
//         List<IQuestion> questions = new();
//         for (int i = 1; i <= count; i++)
//         {
//             questions.Add(CreateTestQuestion($"Question {i}"));
//         }
//
//         return questions;
//     }
//
//     /// <summary>
//     ///     Тесты валидации входных параметров.
//     /// </summary>
//     public class ValidationTests
//     {
//         private readonly ExamPaperGenerator _generator;
//
//         /// <summary>
//         ///     Инициализирует новый экземпляр тестового класса ValidationTests.
//         /// </summary>
//         public ValidationTests()
//         {
//             _generator = new ExamPaperGenerator();
//         }
//
//         /// <summary>
//         ///     Проверяет, что метод <see cref="ExamPaperGenerator.Generate" />
//         ///     выбрасывает <see cref="ArgumentNullException" /> при передаче <c>null</c>
//         ///     в качестве коллекции доступных вопросов.
//         /// </summary>
//         [Fact]
//         public void Generate_WithNullAvailableQuestions_ThrowsArgumentNullException()
//         {
//             // Arrange
//             GenerationSettings settings = new(5, 2);
//
//             // Act & Assert
//             ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
//                 _generator.Generate(null!, settings));
//
//             Assert.Equal("availableQuestions", exception.ParamName);
//         }
//
//         /// <summary>
//         ///     Проверяет, что метод <see cref="ExamPaperGenerator.Generate" />
//         ///     выбрасывает <see cref="ArgumentNullException" /> при передаче <c>null</c>
//         ///     в качестве настроек генерации.
//         /// </summary>
//         [Fact]
//         public void Generate_WithNullSettings_ThrowsArgumentNullException()
//         {
//             // Arrange
//             List<IQuestion> questions = new() { CreateTestQuestion() };
//
//             // Act & Assert
//             ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
//                 _generator.Generate(questions, null!));
//
//             Assert.Equal("settings", exception.ParamName);
//         }
//
//         /// <summary>
//         ///     Проверяет, что метод <see cref="ExamPaperGenerator.Generate" />
//         ///     выбрасывает <see cref="InvalidOperationException" /> когда количество
//         ///     доступных вопросов меньше требуемого количества вопросов на билет.
//         /// </summary>
//         [Fact]
//         public void Generate_WhenNotEnoughQuestions_ThrowsInvalidOperationException()
//         {
//             // Arrange
//             List<IQuestion> questions = new() { CreateTestQuestion("Q1"), CreateTestQuestion("Q2") };
//             GenerationSettings settings = new(5, 3);
//
//             // Act & Assert
//             InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
//                 _generator.Generate(questions, settings));
//
//             Assert.Contains("Недостаточно вопросов", exception.Message);
//             Assert.Contains("Доступно: 2", exception.Message);
//             Assert.Contains("требуется: 3", exception.Message);
//         }
//     }
//
//     /// <summary>
//     ///     Тесты корректности генерации билетов.
//     /// </summary>
//     public class GenerationCorrectnessTests
//     {
//         private readonly ExamPaperGenerator _generator;
//
//         /// <summary>
//         ///     Инициализирует новый экземпляр тестового класса GenerationCorrectnessTests.
//         /// </summary>
//         public GenerationCorrectnessTests()
//         {
//             _generator = new ExamPaperGenerator();
//         }
//
//         /// <summary>
//         ///     Проверяет, что генерируется правильное количество билетов
//         ///     в соответствии с настройками.
//         /// </summary>
//         [Fact]
//         public void Generate_WithValidParameters_ReturnsCorrectNumberOfTickets()
//         {
//             // Arrange
//             List<IQuestion> questions = CreateTestQuestions(10);
//             GenerationSettings settings = new(5, 2);
//
//             // Act
//             List<IExamPaper> tickets = _generator.Generate(questions, settings).ToList();
//
//             // Assert
//             Assert.Equal(5, tickets.Count);
//         }
//
//         /// <summary>
//         ///     Проверяет, что каждый билет содержит правильное количество вопросов
//         ///     в соответствии с настройками.
//         /// </summary>
//         [Fact]
//         public void Generate_EachTicket_ContainsCorrectNumberOfQuestions()
//         {
//             // Arrange
//             List<IQuestion> questions = CreateTestQuestions(20);
//             int expectedQuestionsPerTicket = 3;
//             GenerationSettings settings = new(4, expectedQuestionsPerTicket);
//
//             // Act
//             List<IExamPaper> tickets = _generator.Generate(questions, settings).ToList();
//
//             // Assert
//             foreach (IExamPaper ticket in tickets)
//             {
//                 Assert.Equal(expectedQuestionsPerTicket, ticket.Questions.Count);
//             }
//         }
//
//         /// <summary>
//         ///     Проверяет, что внутри одного билета нет повторяющихся вопросов.
//         /// </summary>
//         [Fact]
//         public void Generate_WithinSingleTicket_NoDuplicateQuestions()
//         {
//             // Arrange
//             List<IQuestion> questions = CreateTestQuestions(15);
//             GenerationSettings settings = new(3, 5);
//
//             // Act
//             List<IExamPaper> tickets = _generator.Generate(questions, settings).ToList();
//
//             // Assert
//             foreach (IExamPaper ticket in tickets)
//             {
//                 IEnumerable<Guid> distinctQuestionIds = ticket.Questions.Select(q => q.Id).Distinct();
//                 Assert.Equal(ticket.Questions.Count, distinctQuestionIds.Count());
//             }
//         }
//
//         /// <summary>
//         ///     Проверяет, что при генерации одного билета все вопросы уникальны.
//         /// </summary>
//         [Fact]
//         public void Generate_SingleTicket_AllQuestionsAreUnique()
//         {
//             // Arrange
//             List<IQuestion> questions = CreateTestQuestions(10);
//             GenerationSettings settings = new(1, 5);
//
//             // Act
//             List<IExamPaper> tickets = _generator.Generate(questions, settings).ToList();
//             IReadOnlyCollection<IQuestion> ticketQuestions = tickets.Single().Questions;
//
//             // Assert
//             IEnumerable<Guid> uniqueIds = ticketQuestions.Select(q => q.Id).Distinct();
//             Assert.Equal(ticketQuestions.Count, uniqueIds.Count());
//         }
//     }
//
//     /// <summary>
//     ///     Тесты случайности и распределения вопросов.
//     /// </summary>
//     public class RandomnessTests
//     {
//         private readonly ExamPaperGenerator _generator;
//
//         /// <summary>
//         ///     Инициализирует новый экземпляр тестового класса RandomnessTests.
//         /// </summary>
//         public RandomnessTests()
//         {
//             _generator = new ExamPaperGenerator();
//         }
//
//         /// <summary>
//         ///     Проверяет, что при многократной генерации билетов с одинаковыми
//         ///     входными данными получаются разные наборы вопросов (благодаря случайности).
//         /// </summary>
//         [Fact]
//         public void Generate_MultipleTimesWithSameInputs_ProducesDifferentResults()
//         {
//             // Arrange
//             List<IQuestion> questions = CreateTestQuestions(10);
//             GenerationSettings settings = new(3, 3);
//
//             // Act
//             List<IExamPaper> firstGeneration = _generator.Generate(questions, settings).ToList();
//             List<IExamPaper> secondGeneration = _generator.Generate(questions, settings).ToList();
//
//             // Assert
//             bool hasDifferences = false;
//             for (int i = 0; i < firstGeneration.Count; i++)
//             {
//                 List<Guid> firstTicketQuestions = firstGeneration[i].Questions.Select(q => q.Id).ToList();
//                 List<Guid> secondTicketQuestions = secondGeneration[i].Questions.Select(q => q.Id).ToList();
//
//                 if (!firstTicketQuestions.SequenceEqual(secondTicketQuestions))
//                 {
//                     hasDifferences = true;
//                     break;
//                 }
//             }
//
//             Assert.True(hasDifferences, "Generations should differ due to randomness");
//         }
//
//         /// <summary>
//         ///     Проверяет, что один и тот же вопрос может встречаться в разных билетах.
//         /// </summary>
//         [Fact]
//         public void Generate_SameQuestion_CanAppearInMultipleTickets()
//         {
//             // Arrange
//             List<IQuestion> questions = CreateTestQuestions(5);
//             GenerationSettings settings = new(5, 3);
//
//             // Act
//             List<IExamPaper> tickets = _generator.Generate(questions, settings).ToList();
//             List<Guid> allQuestionIds = tickets.SelectMany(t => t.Questions.Select(q => q.Id)).ToList();
//             List<Guid> uniqueQuestionIds = allQuestionIds.Distinct().ToList();
//
//             // Assert
//             Assert.True(uniqueQuestionIds.Count < allQuestionIds.Count,
//                 "Questions should repeat across different tickets when question pool is limited");
//         }
//     }
//
//     /// <summary>
//     ///     Тесты граничных случаев.
//     /// </summary>
//     public class EdgeCasesTests
//     {
//         private readonly ExamPaperGenerator _generator;
//
//         /// <summary>
//         ///     Инициализирует новый экземпляр тестового класса EdgeCasesTests.
//         /// </summary>
//         public EdgeCasesTests()
//         {
//             _generator = new ExamPaperGenerator();
//         }
//
//         /// <summary>
//         ///     Проверяет генерацию, когда количество доступных вопросов точно равно
//         ///     требуемому количеству вопросов на билет.
//         /// </summary>
//         [Fact]
//         public void Generate_WhenExactNumberOfQuestions_GeneratesSuccessfully()
//         {
//             // Arrange
//             List<IQuestion> questions = CreateTestQuestions(5);
//             GenerationSettings settings = new(3, 5);
//
//             // Act
//             List<IExamPaper> tickets = _generator.Generate(questions, settings).ToList();
//
//             // Assert
//             Assert.Equal(3, tickets.Count);
//             foreach (IExamPaper ticket in tickets)
//             {
//                 Assert.Equal(5, ticket.Questions.Count);
//                 Assert.Equal(questions.Count, ticket.Questions.Count);
//             }
//         }
//
//         /// <summary>
//         ///     Проверяет генерацию одного билета (TotalTicketsCount = 1).
//         /// </summary>
//         [Fact]
//         public void Generate_SingleTicket_ReturnsOneTicket()
//         {
//             // Arrange
//             List<IQuestion> questions = CreateTestQuestions(10);
//             GenerationSettings settings = new(1, 4);
//
//             // Act
//             List<IExamPaper> tickets = _generator.Generate(questions, settings).ToList();
//
//             // Assert
//             Assert.Single(tickets);
//             Assert.Equal(4, tickets[0].Questions.Count);
//         }
//
//         /// <summary>
//         ///     Проверяет генерацию с максимальными значениями.
//         /// </summary>
//         [Fact]
//         public void Generate_WithLargeNumbers_DoesNotThrowException()
//         {
//             // Arrange
//             List<IQuestion> questions = CreateTestQuestions(100);
//             GenerationSettings settings = new(50, 10);
//
//             // Act
//             Exception? exception = Record.Exception(() =>
//                 _generator.Generate(questions, settings).ToList());
//
//             // Assert
//             Assert.Null(exception);
//         }
//
//         /// <summary>
//         ///     Проверяет, что каждый билет имеет уникальный идентификатор и имя.
//         /// </summary>
//         [Fact]
//         public void Generate_EachTicket_HasUniqueIdAndName()
//         {
//             // Arrange
//             List<IQuestion> questions = CreateTestQuestions(20);
//             GenerationSettings settings = new(10, 3);
//
//             // Act
//             List<IExamPaper> tickets = _generator.Generate(questions, settings).ToList();
//
//             // Assert
//             IEnumerable<Guid> uniqueIds = tickets.Select(t => t.Id).Distinct();
//             IEnumerable<object> uniqueNames = tickets.Select<IExamPaper, object>(t => t.Title).Distinct();
//
//             Assert.Equal(tickets.Count, uniqueIds.Count());
//             Assert.Equal(tickets.Count, uniqueNames.Count());
//         }
//     }
//
//     /// <summary>
//     ///     Тесты, проверяющие, что вопросы выбираются из предоставленного пула.
//     /// </summary>
//     public class QuestionSourceTests
//     {
//         private readonly ExamPaperGenerator _generator;
//
//         /// <summary>
//         ///     Инициализирует новый экземпляр тестового класса QuestionSourceTests.
//         /// </summary>
//         public QuestionSourceTests()
//         {
//             _generator = new ExamPaperGenerator();
//         }
//
//         /// <summary>
//         ///     Проверяет, что все вопросы в билетах берутся из исходной коллекции.
//         /// </summary>
//         [Fact]
//         public void Generate_AllQuestions_FromProvidedCollection()
//         {
//             // Arrange
//             List<IQuestion> sourceQuestions = CreateTestQuestions(15);
//             HashSet<Guid> sourceQuestionIds = sourceQuestions.Select(q => q.Id).ToHashSet();
//             GenerationSettings settings = new(5, 3);
//
//             // Act
//             List<IExamPaper> tickets = _generator.Generate(sourceQuestions, settings).ToList();
//             HashSet<Guid> usedQuestionIds = tickets.SelectMany(t => t.Questions.Select(q => q.Id)).ToHashSet();
//
//             // Assert
//             foreach (Guid questionId in usedQuestionIds)
//             {
//                 Assert.Contains(questionId, sourceQuestionIds);
//             }
//         }
//
//         /// <summary>
//         ///     Проверяет, что метод не модифицирует исходную коллекцию вопросов.
//         /// </summary>
//         [Fact]
//         public void Generate_DoesNotModifyOriginalQuestionsCollection()
//         {
//             // Arrange
//             List<IQuestion> originalQuestions = CreateTestQuestions(10).ToList();
//             int originalCount = originalQuestions.Count;
//             GenerationSettings settings = new(3, 3);
//
//             // Act
//             _generator.Generate(originalQuestions, settings).ToList();
//
//             // Assert
//             Assert.Equal(originalCount, originalQuestions.Count);
//         }
//     }
//
//     /// <summary>
//     ///     Тестовая реализация IQuestion для unit-тестов.
//     /// </summary>
//     private class TestQuestion : IQuestion
//     {
//         public Guid Id { get; init; }
//         public string Text { get; init; } = string.Empty;
//     }
// }