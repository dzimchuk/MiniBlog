using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MiniBlog.Contracts;
using MiniBlog.Contracts.Framework;

namespace MiniBlog.Azure.Util
{
    internal class BlobContainerFactory : IBlobContainerFactory
    {
        private readonly IConfiguration configuration;

        public BlobContainerFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public CloudBlobContainer Create(string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(configuration.Find("blog:contentStorage"));
            var client = storageAccount.CreateCloudBlobClient();
            return client.GetContainerReference(configuration.Find(containerName));
        }
    }
}