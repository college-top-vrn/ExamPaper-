using NetArchTest.Rules;

namespace ExamPaper.Tests.Architecture;

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
        var result = Types.InAssembly(typeof(ExamPaper.Core.Interfaces.IQuestion).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(ServiceNamespace, InfrastructureNamespace)
            .GetResult(); 
        
        Assert.True(result.IsSuccessful, "Слой ядра не должен зависеть от слоя сервиса и инфраструктуры.");
    }
    
    /// <summary>
    /// Тест проверяет, что слой Service не зависит от Infrastructure.
    /// </summary>
    [Fact]
    public void ServiceLayer_ShouldNot_HaveDependencyOn_Infrastructure()
    {
        var result = Types.InAssembly(typeof(ExamPaper.Service.Generator.ExamPaperGenerator).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(InfrastructureNamespace)
            .GetResult(); 
        
        Assert.True(result.IsSuccessful, "Слой сервиса не должен иметь зависимость от слоя инфраструктуры.");
    }
}