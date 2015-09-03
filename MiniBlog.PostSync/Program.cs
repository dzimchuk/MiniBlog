using LightInject;
using Microsoft.Azure.WebJobs;

namespace MiniBlog.PostSync
{
    class Program
    {
        static void Main()
        {
            var container = InitializeLightInject();
            var config = new JobHostConfiguration { JobActivator = new LightInjectJobActivator(container) };

            var host = new JobHost(config);
            host.Call(typeof(SyncJob).GetMethod("SyncPostsAsync"));
        }

        private static IServiceContainer InitializeLightInject()
        {
            var container = new ServiceContainer();
            container.RegisterFrom<Services.Composition.CompositionModule>();
            container.RegisterFrom<Composition.CompositionModule>();

            return container;
        }
    }
}
