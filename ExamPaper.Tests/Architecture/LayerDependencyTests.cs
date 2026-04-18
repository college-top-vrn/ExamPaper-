using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;

using ExamPaper.Core.Interfaces;
using ExamPaper.Core.Models;
using ExamPaper.Infrastructure.Exporter;
using ExamPaper.Infrastructure.Repositories;
using ExamPaper.Service.Generator;

using Xunit;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace ExamPaper.Tests.Architecture;


/// <summary>
    ///     Содержит архитектурные тесты для контроля соблюдения правил Слоистой Архитектуры.
    ///     Гарантирует, что граф зависимостей всегда направлен внутрь — к слою Core,
    ///     и предотвращает протекание деталей реализации (инфраструктуры) в доменную и бизнес-логику.
    /// </summary>
    public class LayerDependencyTests
    {
        private static readonly ArchUnitNET.Domain.Architecture Architecture = new ArchLoader()
            .LoadAssemblies(
                typeof(IQuestion).Assembly,
                typeof(ExamPaperGenerator).Assembly,
                typeof(QuestionRepository).Assembly
            )
            .Build();

        private static readonly IObjectProvider<IType> CoreLayer = Types()
            .That().ResideInAssembly(typeof(IQuestion).Assembly)
            .And().ResideInNamespaceMatching(@"^ExamPaper\.Core(\..*)?$")
            .As("Core Layer");

        private static readonly IObjectProvider<IType> ServiceLayer = Types()
            .That().ResideInAssembly(typeof(ExamPaperGenerator).Assembly)
            .And().ResideInNamespaceMatching(@"^ExamPaper\.Service(\..*)?$")
            .As("Service Layer");

        private static readonly IObjectProvider<IType> InfrastructureLayer = Types()
            .That().ResideInAssembly(typeof(QuestionRepository).Assembly)
            .And().ResideInNamespaceMatching(@"^ExamPaper\.Infrastructure(\..*)?$")
            .As("Infrastructure Layer");

        /// <summary>
        ///     Тест проверяет, что слой Core не зависит от Service и Infrastructure.
        /// </summary>
        [Fact]
        public void CoreLayer_ShouldNot_HaveDependencyOn_ServiceOrInfrastructure()
        {
            var externalLayers = Types()
                .That().Are(ServiceLayer)
                .Or().Are(InfrastructureLayer)
                .As("Service and Infrastructure Layers");

            Types()
                .That().Are(CoreLayer)
                .Should().NotDependOnAny(externalLayers)
                .Because("Слой ядра (домен) должен быть полностью независимым от внешних сервисов и инфраструктуры.")
                .Check(Architecture);
        }

        /// <summary>
        ///     Тест проверяет, что слой Service не зависит от Infrastructure.
        /// </summary>
        [Fact]
        public void ServiceLayer_ShouldNot_HaveDependencyOn_Infrastructure()
        {
            Types()
                .That().Are(ServiceLayer)
                .Should().NotDependOnAny(InfrastructureLayer)
                .Because("Слой сервисов не должен напрямую взаимодействовать с деталями реализации инфраструктуры.")
                .Check(Architecture);
        }

        /// <summary>
        ///     Проверяет, что слой Infrastructure не зависит от слоя Service.
        /// </summary>
        [Fact]
        public void InfrastructureLayer_ShouldNot_HaveDependencyOn_Service()
        {
            Types()
                .That().Are(InfrastructureLayer)
                .Should().NotDependOnAny(ServiceLayer)
                .Because("Слой Infrastructure отвечает только за реализацию контрактов ядра и не должен зависеть от бизнес-логики сервисов.")
                .Check(Architecture);
        }
    }
