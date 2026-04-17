using System;
using System.Reflection;

using ExamPaper.Core.Models;

using Xunit;

namespace ExamPaper.Tests.Unit;

/// <summary>
///     Набор тестов для проверки класса <see cref="GenerationSettings" />.
/// </summary>
/// <remarks>
///     Тесты покрывают следующие аспекты:
///     <list type="bullet">
///         <item>
///             <description>Конструкторы (валидные и невалидные параметры)</description>
///         </item>
///         <item>
///             <description>Пустой конструктор для десериализации</description>
///         </item>
///         <item>
///             <description>Метод валидации <see cref="GenerationSettings.IsValid" /></description>
///         </item>
///         <item>
///             <description>Метод строкового представления <see cref="GenerationSettings.ToString" /></description>
///         </item>
///     </list>
/// </remarks>
public class GenerationSettingsTests
{
    /// <summary>
    ///     Тесты конструктора <see cref="GenerationSettings" />.
    /// </summary>
    public class ConstructorTests
    {
        /// <summary>
        ///     Проверяет, что конструктор с корректными параметрами правильно устанавливает свойства.
        /// </summary>
        /// <param name="totalTickets">Общее количество билетов (положительное число).</param>
        /// <param name="questionsPerTicket">Количество вопросов на билет (положительное число).</param>
        [Theory]
        [InlineData(1, 1)]
        [InlineData(5, 3)]
        [InlineData(100, 10)]
        public void WithValidParameters_ShouldSetProperties(
            int totalTickets,
            int questionsPerTicket
        )
        {
            // Act
            GenerationSettings settings = new(totalTickets, questionsPerTicket);

            // Assert
            Assert.Equal(totalTickets, settings.TotalTicketsCount);
            Assert.Equal(questionsPerTicket, settings.QuestionsPerTicketCount);
        }

        /// <summary>
        ///     Проверяет, что конструктор выбрасывает <see cref="ArgumentException" />
        ///     при передаче невалидного общего количества билетов.
        /// </summary>
        /// <param name="totalTickets">Невалидное количество билетов (0 или отрицательное).</param>
        /// <param name="questionsPerTicket">Валидное количество вопросов на билет.</param>
        [Theory]
        [InlineData(0, 5)]
        [InlineData(-1, 5)]
        [InlineData(-10, 5)]
        public void WithInvalidTotalTicketsCount_ShouldThrowArgumentException(
            int totalTickets,
            int questionsPerTicket
        )
        {
            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                new GenerationSettings(totalTickets, questionsPerTicket)
            );

            Assert.Equal("totalTicketsCount", exception.ParamName);
        }

        /// <summary>
        ///     Проверяет, что конструктор выбрасывает <see cref="ArgumentException" />
        ///     при передаче невалидного количества вопросов на билет.
        /// </summary>
        /// <param name="totalTickets">Валидное количество билетов.</param>
        /// <param name="questionsPerTicket">Невалидное количество вопросов (0 или отрицательное).</param>
        [Theory]
        [InlineData(5, 0)]
        [InlineData(5, -1)]
        [InlineData(5, -100)]
        public void WithInvalidQuestionsPerTicketCount_ShouldThrowArgumentException(
            int totalTickets,
            int questionsPerTicket
        )
        {
            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                new GenerationSettings(totalTickets, questionsPerTicket)
            );

            Assert.Equal("questionsPerTicketCount", exception.ParamName);
        }
    }

    /// <summary>
    ///     Тесты пустого конструктора для десериализации.
    /// </summary>
    public class DefaultConstructorTests
    {
        /// <summary>
        ///     Проверяет, что пустой конструктор создаёт экземпляр со значениями по умолчанию (0).
        /// </summary>
        [Fact]
        public void ShouldCreateInstanceWithDefaultValues()
        {
            // Act
            GenerationSettings settings = new();

            // Assert
            Assert.Equal(0, settings.TotalTicketsCount);
            Assert.Equal(0, settings.QuestionsPerTicketCount);
        }
    }

    /// <summary>
    ///     Тесты метода <see cref="GenerationSettings.IsValid" />.
    /// </summary>
    public class IsValidMethodTests
    {
        /// <summary>
        ///     Проверяет, что <see cref="GenerationSettings.IsValid" /> возвращает <c>true</c>
        ///     для валидных настроек, созданных через конструктор с параметрами.
        /// </summary>
        /// <param name="totalTickets">Общее количество билетов (положительное).</param>
        /// <param name="questionsPerTicket">Количество вопросов на билет (положительное).</param>
        /// <param name="expected">Ожидаемый результат (всегда true).</param>
        [Theory]
        [InlineData(1, 1, true)]
        [InlineData(5, 3, true)]
        [InlineData(100, 10, true)]
        public void WithValidValues_ShouldReturnTrue(
            int totalTickets,
            int questionsPerTicket,
            bool expected
        )
        {
            // Arrange
            GenerationSettings settings = new(totalTickets, questionsPerTicket);

            // Act
            bool isValid = settings.IsValid();

            // Assert
            Assert.Equal(expected, isValid);
        }

        /// <summary>
        ///     Проверяет, что <see cref="GenerationSettings.IsValid" /> возвращает <c>false</c>
        ///     для невалидных настроек (используется рефлексия для установки значений).
        /// </summary>
        /// <param name="totalTickets">Невалидное количество билетов.</param>
        /// <param name="questionsPerTicket">Невалидное количество вопросов.</param>
        /// <param name="expected">Ожидаемый результат (всегда false).</param>
        /// <remarks>
        ///     Тест использует рефлексию для установки private полей, так как
        ///     невалидные значения нельзя передать через публичный конструктор.
        /// </remarks>
        [Theory]
        [InlineData(0, 5, false)]
        [InlineData(5, 0, false)]
        [InlineData(0, 0, false)]
        public void WithInvalidValues_ShouldReturnFalse(
            int totalTickets,
            int questionsPerTicket,
            bool expected
        )
        {
            // Arrange
            GenerationSettings settings = new();

            // Устанавливаем значения через рефлексию
            Type type = settings.GetType();
            FieldInfo? totalTicketsField = type.GetField(
                "<TotalTicketsCount>k__BackingField",
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            FieldInfo? questionsField = type.GetField(
                "<QuestionsPerTicketCount>k__BackingField",
                BindingFlags.Instance | BindingFlags.NonPublic
            );

            totalTicketsField?.SetValue(settings, totalTickets);
            questionsField?.SetValue(settings, questionsPerTicket);

            // Act
            bool isValid = settings.IsValid();

            // Assert
            Assert.Equal(expected, isValid);
        }
    }

    /// <summary>
    ///     Тесты метода <see cref="GenerationSettings.ToString" />.
    /// </summary>
    public class ToStringMethodTests
    {
        /// <summary>
        ///     Проверяет, что <see cref="GenerationSettings.ToString" /> возвращает
        ///     корректно отформатированную строку для валидных настроек.
        /// </summary>
        /// <param name="totalTickets">Общее количество билетов.</param>
        /// <param name="questionsPerTicket">Количество вопросов на билет.</param>
        /// <param name="expected">Ожидаемая строка в формате "Билетов: X, Вопросов на билет: Y".</param>
        [Theory]
        [InlineData(1, 1, "Билетов: 1, Вопросов на билет: 1")]
        [InlineData(10, 5, "Билетов: 10, Вопросов на билет: 5")]
        [InlineData(100, 20, "Билетов: 100, Вопросов на билет: 20")]
        public void WithValidValues_ShouldReturnFormattedString(
            int totalTickets,
            int questionsPerTicket,
            string expected
        )
        {
            // Arrange
            GenerationSettings settings = new(totalTickets, questionsPerTicket);

            // Act
            string result = settings.ToString();

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        ///     Проверяет, что <see cref="GenerationSettings.ToString" /> возвращает
        ///     строку с нулевыми значениями для экземпляра, созданного через пустой конструктор.
        /// </summary>
        [Fact]
        public void WithDefaultValues_ShouldReturnFormattedStringWithZeros()
        {
            // Arrange
            GenerationSettings settings = new();

            // Act
            string result = settings.ToString();

            // Assert
            Assert.Equal("Билетов: 0, Вопросов на билет: 0", result);
        }
    }
}