using System;
using LiteCqrs.Domain;
using LiteCqrs.TestScenario.CommandHandlers;
using LiteCqrs.TestScenario.Denormalizers;
using LiteCqrs.TestScenario.EventHandlers;
using LiteCqrs.TestScenario.SubCommandHandlers.SubA;
using LiteCqrs.TestScenario.SubDenormalizers.SubA;
using LiteCqrs.TestScenario.SubEventHandlers.SubA;

namespace LiteCqrs.Specifications
{
    public static class TestContextFactory
    {
        public static ITestContext CreateDefault()
        {
            Func<IDomainRepository> domainRepositoryFn = null;

            var builder = new CqrsRuntimeBuilder();
            builder.CommandHandlerContainerFactory = t => Activator.CreateInstance(t, domainRepositoryFn.Invoke());
            builder.EventHandlerContainerFactory = Activator.CreateInstance;

            var commandHandlersConfig = new AssemblyScanConfig(typeof(MyCommandHandlers).Assembly);
            var subCommandHandlersConfig = new AssemblyScanConfig(typeof (MySubCommandHandlers).Assembly)
            {
                NamespaceFilter = n => n.Contains("SubCommandHandlers")
            };
            var eventHandlersConfig = new AssemblyScanConfig(typeof(MyEventHandlers).Assembly);
            var subEventHandlersConfig = new AssemblyScanConfig(typeof(MySubEventHandlers).Assembly)
            {
                NamespaceFilter = n => n.Contains("SubEventHandlers")
            };
            var denormalizersConfig = new AssemblyScanConfig(typeof(MyDenormalizers).Assembly)
            {
                NamespaceFilter = n => n != null && n.EndsWith("Denormalizers")
            };
            var subDenormalizersConfig = new AssemblyScanConfig(typeof(MySubDenormalizers).Assembly)
            {
                NamespaceFilter = n => n != null && n.Contains("SubDenormalizers")
            };

            var cqrsRuntime = builder.Build(
                new[] { commandHandlersConfig, subCommandHandlersConfig },
                new[] { eventHandlersConfig, subEventHandlersConfig, denormalizersConfig, subDenormalizersConfig });
            domainRepositoryFn = cqrsRuntime.GetDomainRepository;

            return new TestContext(cqrsRuntime);
        }
    }
}