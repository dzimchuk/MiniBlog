using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using MiniBlog.Contracts;
using MiniBlog.Contracts.Framework;
using MiniBlog.WebJobs.Common.Model;

namespace MiniBlog.ImageOptimizer
{
    public class OptimizerJob
    {
        private const string OptimizedContainerKey = "blog:optimizedContentContainer";
        private readonly IConfiguration configuration;

        public OptimizerJob(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task ProcessImageAsync([BlobTrigger("%blog:contentContainer%/{blobName}")] ICloudBlob blob,
                                            [Blob("%blog:optimizedContentContainer%/{blobName}")] CloudBlockBlob optimizedImage,
                                            CloudStorageAccount storageAccount,
                                            TextWriter log)
        {
            using (var compressor = new Compressor(blob))
            {
                if (compressor.IsSupported)
                {
                    var result = await compressor.CompressAsync();
                    if (result.Saving > 0)
                    {
                        optimizedImage.Properties.ContentType = blob.Properties.ContentType;
                        optimizedImage.Properties.CacheControl = blob.Properties.CacheControl;

                        await result.UploadResultFileAsync(optimizedImage);
                        await AddMapRecordAsync(blob, optimizedImage, storageAccount);

                        await log.WriteLineAsync($"{blob.Name} has been optimized.");
                        await log.WriteLineAsync(result.ToString());
                    }
                    else
                    {
                        await log.WriteLineAsync($"{blob.Name} is already optimized.");
                    }
                }
                else
                {
                    await log.WriteLineAsync($"{blob.Name} is not a supported file type.");
                }
            }
        }

        public static void LogPoisonBlob([QueueTrigger("webjobs-blobtrigger-poison")] PoisonBlobMessage message, TextWriter logger)
        {
            logger.WriteLine("FunctionId: {0}", message.FunctionId);
            logger.WriteLine("BlobType: {0}", message.BlobType);
            logger.WriteLine("ContainerName: {0}", message.ContainerName);
            logger.WriteLine("BlobName: {0}", message.BlobName);
            logger.WriteLine("ETag: {0}", message.ETag);
        }

        private async Task AddMapRecordAsync(ICloudBlob blob, CloudBlockBlob optimizedImage, CloudStorageAccount storageAccount)
        {
            var link = new ImageLink
                       {
                           PartitionKey = "Image",
                           RowKey = GetRowKey(blob),
                           OriginalImagePath = GetBlobPath(blob, Constants.FileContainerKey),
                           OptimizedImagePath = GetBlobPath(optimizedImage, OptimizedContainerKey)
                       };

            var client = storageAccount.CreateCloudTableClient();
            var table = client.GetTableReference(configuration.Find(Constants.OptimizedImageMap));

            var operation = TableOperation.InsertOrReplace(link);
            await table.ExecuteAsync(operation);
        }

        private string GetBlobPath(ICloudBlob blob, string containerKey)
        {
            return $"{configuration.Find(containerKey)}/{blob.Name}";
        }

        private static string GetRowKey(ICloudBlob blob)
        {
            return blob.Name.Replace("/", "{slash}").Replace("\\", "{backslash}").Replace("#", "{hash}").Replace("?", "{question}");
        }
    }
}
