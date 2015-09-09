using LightInject;
using Microsoft.Azure.WebJobs;
using MiniBlog.WebJobs.Common;

namespace MiniBlog.Backup
{
    class Program
    {
        static void Main()
        {
            var container = InitializeLightInject();
            var configurationFactory = new JobHostConfigurationFactory(container);

            var host = new JobHost(configurationFactory.Create());
            host.Call(typeof(BackupJob).GetMethod("BackupBlogAsync"));
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
