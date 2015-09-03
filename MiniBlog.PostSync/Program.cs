using LightInject;
using Microsoft.Azure.WebJobs;
using MiniBlog.Contracts;

namespace MiniBlog.PostSync
{
    class Program
    {
        static void Main()
        {
            var container = InitializeLightInject();
            var configuration = container.GetInstance<IConfiguration>();

            var hostConfiguration = new JobHostConfiguration
                                    {
                                        JobActivator = new LightInjectJobActivator(container),
                                        DashboardConnectionString = configuration.Find("AzureWebJobsDashboard"),
                                        StorageConnectionString = configuration.Find("AzureWebJobsStorage")
                                    };

            var host = new JobHost(hostConfiguration);
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
