using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using ArchUnitNET.Fluent; 

using ExamPaper.Core.Models;
using ExamPaper.Infrastructure.Exporter;
using ExamPaper.Infrastructure.Repositories;
using ExamPaper.Service.Generator;

using Xunit;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace ExamPaper.Tests.Architecture;

/// <summary>
///     Содержит архитектурные тесты для проверки соблюдения правил проектирования,
///     правильного использования модификаторов доступа и защиты инкапсуляции.
/// </summary>
public class DesignRulesTests
{
    private static readonly ArchUnitNET.Domain.Architecture _architecture = new ArchLoader()
        .LoadAssemblies(typeof(Question).Assembly,
            typeof(QuestionRepository).Assembly,
            typeof(ExamPaperGenerator).Assembly,
            typeof(JsonExamExporter).Assembly,
            typeof(PdfExamExporter).Assembly,
            typeof(Core.Models.ExamPaper).Assembly).Build();

    private static readonly IObjectProvider<IType> _coreLayer = Types()
        .That()
        .ResideInAssembly(typeof(Question).Assembly).And().ResideInNamespaceMatching(@"^ExamPaper\.Core(\..*)?$")
        .As("Core Layer");

    private static readonly IObjectProvider<IType> _infrastructureLayer = Types()
        .That()
        .ResideInAssembly(typeof(QuestionRepository).Assembly).And()
        .ResideInNamespaceMatching(@"^ExamPaper\.Infrastructure(\..*)?$")
        .As("Infrastructure Layer");

    /// <summary>
    ///     Проверяет, что все классы доменных моделей в слое Core
    ///     помечены модификатором 'sealed'. Это предотвращает непредсказуемое наследование
    ///     и изменение базового поведения фундаментальных сущностей проекта.
    /// </summary>
    [Fact]
    public void CoreModels_Should_BeSealed()
    {
        Classes()
            .That().Are(_coreLayer).And()
            .AreNotAbstract()
            .And().AreNotSealed()
            .Should().NotExist()
            .Because("Все модели в слое Core должны быть sealed")
            .Check(_architecture);
    }


    /// <summary>
    ///     Проверяет, что классы конкретных реализаций в слое Infrastructure
    ///     (такие как репозитории и экспортеры) являются 'sealed'.
    ///     Инфраструктурные классы предназначены для выполнения конкретной работы,
    ///     а не для того, чтобы служить базовыми классами для других.
    /// </summary>
    [Fact]
    public void InfrastructureImplementations_Should_BeSealed()
    {
        Classes().That()
            .Are(_infrastructureLayer)
            .And().AreNotAbstract().And().AreNotSealed()
            .Should().NotExist().Because("Реализации в слое Infrastructure должны быть помечены как sealed.")
            .Check(_architecture);
    }


    /// <summary>
    ///     Проверяет отсутствие статических классов в слое Core.
    ///     Использование статического состояния в доменной логике нарушает принципы ООП
    ///     и затрудняет тестирование. Вместо них следует использовать интерфейсы и Иньекцию зависимотей.
    /// </summary>
    [Fact]
    public void Classes_In_Core_ShouldNot_Be_Static()
    {
        Classes()
            .That()
            .Are(_coreLayer)
            .And().AreAbstract()
            .And().AreSealed()
            .As("Static Classes")
            .Should().NotExist()
            .Because("Слой Core не должен содержать статических классов (используйте интерфейсы и DI)")
            .Check(_architecture);
    }

    /// <summary>
    ///     Проверяет что все контаркты определены в Core
    /// </summary>
    [Fact]
    public void AllInterfaces_Should_Reside_In_Core()
    {
        Interfaces()
            .That()
            .ResideInNamespaceMatching(@"^(?!ExamPaper\.Core\.Interfaces(\..*)?$).*$")
            .Should().NotExist()
            .Because("Архитектура требует, чтобы все контракты системы были централизованы в ExamPaper.Core.Interfaces")
            .Check(_architecture);
    }
}