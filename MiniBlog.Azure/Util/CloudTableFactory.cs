using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using MiniBlog.Contracts;
using MiniBlog.Contracts.Framework;

namespace MiniBlog.Azure.Util
{
    internal class CloudTableFactory : ICloudTableFactory
    {
        private readonly IConfiguration configuration;

        public CloudTableFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public CloudTable Create(string tableKey)
        {
            var storageAccount = CloudStorageAccount.Parse(configuration.Find(Constants.ContentStorageKey));
            var client = storageAccount.CreateCloudTableClient();
            return client.GetTableReference(configuration.Find(tableKey));
        }
    }
}