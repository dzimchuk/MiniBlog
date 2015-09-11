using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Blob;
using MiniBlog.Contracts;
using MiniBlog.Contracts.Framework;

namespace MiniBlog.ImageOptimizer
{
    public class OptimizerJob
    {
        private const string OptimizedContainerKey = "blog:optimizedContentContainer";

        private readonly Compressor compressor = new Compressor();
        private readonly IConfiguration configuration;

        public OptimizerJob(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task ProcessImageAsync([BlobTrigger("%blog:contentContainer%/{blobName}")] ICloudBlob blob,
                                            [Blob("%blog:optimizedContentContainer%/{blobName}", FileAccess.Write)] CloudBlockBlob optimizedImage,
                                            [Table("%blog:optimizedImageMap%")] IAsyncCollector<ImageLink> collector,
                                            TextWriter log)
        {
            await log.WriteLineAsync($"Optimizing blob {blob.Name}...");

            using (var result = await compressor.CompressAsync(blob))
            {
                if (result.Saving >= 0) // TODO: should be strictly more than (>)
                {
                    await result.UploadResultFileAsync(optimizedImage);
                    var link = new ImageLink
                               {
                                   PartitionKey = GetBlobPath(blob, Constants.FileContainerKey),
                                   RowKey = string.Empty,
                                   OptimizedImagePath = GetBlobPath(optimizedImage, OptimizedContainerKey)
                               };

                    await collector.AddAsync(link);
                }
            }

            await log.WriteLineAsync("Done.");
        }

        private string GetBlobPath(ICloudBlob blob, string containerKey)
        {
            return $"{configuration.Find(containerKey)}/{blob.Name}";
        }
    }
}
