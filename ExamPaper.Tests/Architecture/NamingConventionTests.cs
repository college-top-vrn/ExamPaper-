using NetArchTest.Rules;

using Xunit;

namespace ExamPaper.Tests.Architecture;

/// <summary>
/// Содержит архитектурные тесты для проверки соблюдения командой разработчиков 
/// единых соглашений об именовании классов, методов и других компонентов системы.
/// </summary>
public class NamingConventionTests
{
    private readonly System.Reflection.Assembly _coreAssembly = typeof(ExamPaper.Core.Models.Question).Assembly;

    private readonly System.Reflection.Assembly _serviceAssembly =
        typeof(ExamPaper.Service.Generator.ExamPaperGenerator).Assembly;

    private readonly System.Reflection.Assembly _infrastructureAssembly =
        typeof(ExamPaper.Infrastructure.Repositories.QuestionRepository).Assembly;

    /// <summary>
    /// Проверяет, что абсолютно все интерфейсы в проекте (Core, Service, Infrastructure) 
    /// начинаются с заглавной буквы 'I' (например, IQuestion, IExamPaper).
    /// </summary>
    [Fact]
    public void AllInterfaces_Should_StartWith_I()
    {
        var result = Types.InAssemblies([_coreAssembly, _serviceAssembly, _infrastructureAssembly])
            .That()
            .AreInterfaces()
            .Should()
            .HaveNameStartingWith("I")
            .GetResult();

        Assert.True(result.IsSuccessful, "Все интерфейсы должны начинаться с заглавной буквы 'I'.");
    }

    /// <summary>
    /// Проверяет, что все классы, находящиеся в пространстве имен Repositories слоя Infrastructure, 
    /// имеют обязательный суффикс 'Repository' в названии.
    /// </summary>
    [Fact]
    public void Repositories_Should_Have_RepositorySuffix()
    {
        var result = Types.InAssembly(_infrastructureAssembly)
            .That()
            .ResideInNamespace("ExamPaper.Infrastructure.Repositories")
            .And()
            .AreClasses()
            .Should()
            .HaveNameEndingWith("Repository")
            .GetResult();

        Assert.True(result.IsSuccessful, "Все репозитории должны иметь суффикс 'Repository'.");
    }


    /// <summary>
    /// Проверяет, что все классы-фабрики, находящиеся в слое Service, 
    /// имеют обязательный суффикс 'Factory' в названии.
    /// </summary>
    [Fact]
    public void Factories_Should_Have_FactorySuffix()
    {
        var result = Types.InAssembly(_serviceAssembly)
            .That()
            .ResideInNamespace("ExamPaper.Service.Factories")
            .And()
            .AreClasses()
            .Should()
            .HaveNameEndingWith("Factory")
            .GetResult();

        Assert.True(result.IsSuccessful, "Все фабрики должны иметь суффикс 'Factory'.");
    }

    /// <summary>
    /// Проверяет, что все классы экспорта данных, находящиеся в слое Infrastructure, 
    /// имеют обязательный суффикс 'Exporter' в названии.
    /// </summary>
    [Fact]
    public void Exporters_Should_Have_ExporterSuffix()
    {
        var result = Types.InAssembly(_infrastructureAssembly)
            .That()
            .ResideInNamespace("ExamPaper.Infrastructure.Exporter")
            .And()
            .AreClasses()
            .Should()
            .HaveNameEndingWith("Exporter")
            .GetResult();

        Assert.True(result.IsSuccessful, "Все экспортеры должны иметь суффикс 'Exporter'.");
    }
}