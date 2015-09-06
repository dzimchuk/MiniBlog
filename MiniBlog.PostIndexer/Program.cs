using LightInject;
using Microsoft.Azure.WebJobs;
using MiniBlog.Services.Composition;
using MiniBlog.WebJobs.Common;

namespace MiniBlog.PostIndexer
{
    internal class Program
    {
        private static void Main()
        {
            var container = InitializeLightInject();
            var configurationFactory = new JobHostConfigurationFactory(container);

            var host = new JobHost(configurationFactory.Create());
            host.RunAndBlock();
        }

        private static IServiceContainer InitializeLightInject()
        {
            var container = new ServiceContainer();
            container.RegisterFrom<CompositionModule>();
            container.RegisterFrom<Search.Composition.CompositionModule>();
            container.RegisterFrom<Composition.CompositionModule>();

            return container;
        }
    }
}