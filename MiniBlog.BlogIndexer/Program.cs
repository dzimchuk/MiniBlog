using LightInject;
using Microsoft.Azure.WebJobs;
using MiniBlog.WebJobs.Common;

namespace MiniBlog.BlogIndexer
{
    class Program
    {
        static void Main()
        {
            var container = InitializeLightInject();
            var configurationFactory = new JobHostConfigurationFactory(container);

            var host = new JobHost(configurationFactory.Create());
            host.Call(typeof(IndexerJob).GetMethod("IndexAsync"));
        }

        private static IServiceContainer InitializeLightInject()
        {
            var container = new ServiceContainer();
            container.RegisterFrom<MiniBlog.Services.Composition.CompositionModule>();
            container.RegisterFrom<MiniBlog.Search.Composition.CompositionModule>();
            container.RegisterFrom<Composition.CompositionModule>();

            return container;
        }
    }
}
