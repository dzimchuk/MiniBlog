using LightInject;
using Microsoft.Azure.WebJobs;
using MiniBlog.Contracts.Framework;

namespace MiniBlog.WebJobs.Common
{
    public class JobHostConfigurationFactory
    {
        private readonly IServiceContainer container;

        public JobHostConfigurationFactory(IServiceContainer container)
        {
            this.container = container;
        }

        public JobHostConfiguration Create()
        {
            var configuration = container.GetInstance<IConfiguration>();

            return new JobHostConfiguration
                   {
                       JobActivator = new LightInjectJobActivator(container),
                       DashboardConnectionString = configuration.Find("AzureWebJobsDashboard"),
                       StorageConnectionString = configuration.Find("AzureWebJobsStorage")
                   };
        }
    }
}