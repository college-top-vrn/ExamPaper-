using System;
using System.Reflection;

using ExamPaper.Core.Models;

using Xunit;

namespace ExamPaper.Tests.Unit;

public class GenerationSettingsTests
{
    public class ConstructorTests
    {
        [Theory]
        [InlineData(1, 1)]
        [InlineData(5, 3)]
        [InlineData(100, 10)]
        public void WithValidParameters_ShouldSetProperties(int totalTickets, int questionsPerTicket)
        {
            GenerationSettings settings = new(totalTickets, questionsPerTicket);

            Assert.Equal(totalTickets, settings.TotalTicketsCount);
            Assert.Equal(questionsPerTicket, settings.QuestionsPerTicketCount);
        }

        [Theory]
        [InlineData(0, 5)]
        [InlineData(-1, 5)]
        [InlineData(-10, 5)]
        public void WithInvalidTotalTicketsCount_ShouldThrowArgumentException(int totalTickets, int questionsPerTicket)
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                new GenerationSettings(totalTickets, questionsPerTicket));

            Assert.Equal("totalTicketsCount", exception.ParamName);
        }

        [Theory]
        [InlineData(5, 0)]
        [InlineData(5, -1)]
        [InlineData(5, -100)]
        public void WithInvalidQuestionsPerTicketCount_ShouldThrowArgumentException(int totalTickets,
            int questionsPerTicket)
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                new GenerationSettings(totalTickets, questionsPerTicket));

            Assert.Equal("questionsPerTicketCount", exception.ParamName);
        }
    }

    public class DefaultConstructorTests
    {
        [Fact]
        public void ShouldCreateInstanceWithDefaultValues()
        {
            GenerationSettings settings = new();

            Assert.Equal(0, settings.TotalTicketsCount);
            Assert.Equal(0, settings.QuestionsPerTicketCount);
        }
    }

    public class IsValidMethodTests
    {
        // Только валидные значения через конструктор
        [Theory]
        [InlineData(1, 1, true)]
        [InlineData(5, 3, true)]
        [InlineData(100, 10, true)]
        public void WithValidValues_ShouldReturnTrue(int totalTickets, int questionsPerTicket, bool expected)
        {
            GenerationSettings settings = new(totalTickets, questionsPerTicket);
            bool isValid = settings.IsValid();
            Assert.Equal(expected, isValid);
        }

        // Невалидные значения через пустой конструктор
        [Theory]
        [InlineData(0, 5, false)]
        [InlineData(5, 0, false)]
        [InlineData(0, 0, false)]
        public void WithInvalidValues_ShouldReturnFalse(int totalTickets, int questionsPerTicket, bool expected)
        {
            // Используем пустой конструктор
            GenerationSettings settings = new();

            // Устанавливаем значения через рефлексию
            Type type = settings.GetType();
            FieldInfo? totalTicketsField = type.GetField("<TotalTicketsCount>k__BackingField",
                BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo? questionsField = type.GetField("<QuestionsPerTicketCount>k__BackingField",
                BindingFlags.Instance | BindingFlags.NonPublic);

            totalTicketsField?.SetValue(settings, totalTickets);
            questionsField?.SetValue(settings, questionsPerTicket);

            bool isValid = settings.IsValid();
            Assert.Equal(expected, isValid);
        }
    }

    public class ToStringMethodTests
    {
        [Theory]
        [InlineData(1, 1, "Билетов: 1, Вопросов на билет: 1")]
        [InlineData(10, 5, "Билетов: 10, Вопросов на билет: 5")]
        [InlineData(100, 20, "Билетов: 100, Вопросов на билет: 20")]
        public void WithValidValues_ShouldReturnFormattedString(int totalTickets, int questionsPerTicket,
            string expected)
        {
            GenerationSettings settings = new(totalTickets, questionsPerTicket);
            string result = settings.ToString();
            Assert.Equal(expected, result);
        }

        [Fact]
        public void WithDefaultValues_ShouldReturnFormattedStringWithZeros()
        {
            GenerationSettings settings = new();
            string result = settings.ToString();
            Assert.Equal("Билетов: 0, Вопросов на билет: 0", result);
        }
    }
}