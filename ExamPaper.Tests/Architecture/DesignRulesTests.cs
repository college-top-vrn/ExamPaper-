using NetArchTest.Rules;
using Xunit;

namespace ExamPaper.Tests.Architecture;

/// <summary>
/// Содержит архитектурные тесты для проверки соблюдения правил проектирования,
///  правильного использования модификаторов доступа и защиты инкапсуляции.
/// </summary>
public class DesignRulesTests
{
    private readonly System.Reflection.Assembly _coreAssembly =
        typeof(ExamPaper.Core.Models.Question).Assembly;

    private readonly System.Reflection.Assembly _infrastructureAssembly =
        typeof(ExamPaper.Infrastructure.Repositories.QuestionRepository).Assembly;

    /// <summary>
    /// Проверяет, что все классы доменных моделей (Domain Models) в слое Core
    /// помечены модификатором 'sealed'. Это предотвращает непредсказуемое наследование
    /// и изменение базового поведения фундаментальных сущностей проекта.
    /// </summary>
    [Fact]
    public void CoreModels_Should_BeSealed()
    {
        var result = Types
            .InAssembly(_coreAssembly)
            .That()
            .ResideInNamespace("ExamPaper.Core.Models")
            .And()
            .AreClasses()
            .And()
            .AreNotAbstract()
            .Should()
            .BeSealed()
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            "Модели в слое Core (папка Models) должны быть помечены как sealed."
        );
    }

    /// <summary>
    /// Проверяет, что классы конкретных реализаций в слое Infrastructure
    /// (такие как репозитории и экспортеры) являются 'sealed'.
    /// Инфраструктурные классы предназначены для выполнения конкретной работы,
    /// а не для того, чтобы служить базовыми классами для других.
    /// </summary>
    [Fact]
    public void InfrastructureImplementations_Should_BeSealed()
    {
        var result = Types
            .InAssembly(_infrastructureAssembly)
            .That()
            .AreClasses()
            .And()
            .AreNotAbstract()
            .Should()
            .BeSealed()
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            "Реализации в слое Infrastructure должны быть помечены как sealed."
        );
    }

    /// <summary>
    /// Проверяет отсутствие статических классов в слое Core.
    /// Использование статического состояния в доменной логике нарушает принципы ООП
    /// и затрудняет тестирование. Вместо них следует использовать интерфейсы и Иньекцию зависимотей.
    /// </summary>
    [Fact]
    public void Classes_In_Core_ShouldNot_Be_Static()
    {
        var result = Types
            .InAssembly(_coreAssembly)
            .That()
            .AreClasses()
            .ShouldNot()
            .BeStatic()
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            "Слой Core не должен содержать статических классов (используйте интерфейсы и DI)."
        );
    }

    /// <summary>
    /// Проверяет что все контаркты определены в Core
    /// </summary>
    [Fact]
    public void AllInterfaces_Should_Reside_In_Core()
    {
        var coreAssembly = typeof(ExamPaper.Core.Models.Question).Assembly;
        var serviceAssembly = typeof(ExamPaper.Service.Generator.ExamPaperGenerator).Assembly;
        var infrastructureAssembly =
            typeof(ExamPaper.Infrastructure.Repositories.QuestionRepository).Assembly;

        var result = Types
            .InAssemblies([coreAssembly, serviceAssembly, infrastructureAssembly])
            .That()
            .AreInterfaces()
            .Should()
            .ResideInNamespaceStartingWith("ExamPaper.Core")
            .GetResult();

        Assert.True(result.IsSuccessful, "Все контракты системы должны располагаться в слое Core");
    }
}
