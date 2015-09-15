using LightInject;
using Microsoft.Azure.WebJobs;
using MiniBlog.Services.Composition;
using MiniBlog.WebJobs.Common;

namespace MiniBlog.ImageOptimizer
{
    internal class Program
    {
        private static void Main()
        {
            var container = InitializeLightInject();
            var configurationFactory = new JobHostConfigurationFactory(container);

            var configuration = configurationFactory.Create();
            configuration.Queues.BatchSize = 1;

            var host = new JobHost(configuration);
            host.RunAndBlock();
        }

        private static IServiceContainer InitializeLightInject()
        {
            var container = new ServiceContainer();
            container.RegisterFrom<CompositionModule>();
            container.RegisterFrom<Composition.CompositionModule>();

            return container;
        }
    }
}