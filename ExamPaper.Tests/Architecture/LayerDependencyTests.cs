using NetArchTest.Rules;
using Xunit;

namespace ExamPaper.Tests.Architecture;

/// <summary>
/// Содержит архитектурные тесты для контроля соблюдения правил Слоистой Архитектуры
/// Гарантирует, что граф зависимостей всегда направлен внутрь — к слою Core,
/// и предотвращает протекание деталей реализации (инфраструктуры) в доменную и бизнес-логику.
/// </summary>
public class LayerDependencyTests
{
    private const string CoreNamespace = "ExamPaper.Core";
    private const string ServiceNamespace = "ExamPaper.Service";
    private const string InfrastructureNamespace = "ExamPaper.Infrastructure";

    /// <summary>
    /// Тест проверяет, что слой Core не зависит от Service и Infrastructure.
    /// </summary>
    [Fact]
    public void CoreLayer_ShouldNot_HaveDependencyOn_ServiceOrInfrastructure()
    {
        var result = Types
            .InAssembly(typeof(ExamPaper.Core.Interfaces.IQuestion).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(ServiceNamespace, InfrastructureNamespace)
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            "Слой ядра не должен зависеть от слоя сервиса и инфраструктуры."
        );
    }

    /// <summary>
    /// Тест проверяет, что слой Service не зависит от Infrastructure.
    /// </summary>
    [Fact]
    public void ServiceLayer_ShouldNot_HaveDependencyOn_Infrastructure()
    {
        var result = Types
            .InAssembly(typeof(ExamPaper.Service.Generator.ExamPaperGenerator).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(InfrastructureNamespace)
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            "Слой сервиса не должен иметь зависимость от слоя инфраструктуры."
        );
    }

    /// <summary>
    /// Проверяет, что слой Infrastructure не зависит от слоя Service.
    /// </summary>
    [Fact]
    public void InfrastructureLayer_ShouldNot_HaveDependencyOn_Service()
    {
        var result = Types
            .InAssembly(typeof(ExamPaper.Infrastructure.Repositories.QuestionRepository).Assembly)
            .ShouldNot()
            .HaveDependencyOn(ServiceNamespace)
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            "Слой Infrastructure не должен иметь зависимостей от слоя Service."
        );
    }
}
