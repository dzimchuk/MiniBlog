using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using MiniBlog.Contracts;
using MiniBlog.Contracts.Framework;

namespace MiniBlog.Azure.Util
{
    internal class CloudQueueFactory : ICloudQueueFactory
    {
        private readonly IConfiguration configuration;

        public CloudQueueFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public CloudQueue Create(string queueKey)
        {
            var storageAccount = CloudStorageAccount.Parse(configuration.Find(Constants.ContentStorageKey));
            var client = storageAccount.CreateCloudQueueClient();
            return client.GetQueueReference(configuration.Find(queueKey));
        }
    }
}