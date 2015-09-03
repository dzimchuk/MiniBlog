using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MiniBlog.Contracts;

namespace MiniBlog.PostSync
{
    public class SyncJob
    {
        private readonly ILocalStorage localStorage;
        private readonly IConfiguration configuration;

        public SyncJob(ILocalStorage localStorage, IConfiguration configuration)
        {
            this.localStorage = localStorage;
            this.configuration = configuration;
        }

        [NoAutomaticTrigger]
        public async Task SyncPostsAsync(CloudStorageAccount storageAccount, TextWriter log)
        {
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(configuration.Find("blog:postContainer"));

            await SyncPostsAsync(container, log);
        }

        private async Task SyncPostsAsync(CloudBlobContainer container, TextWriter log)
        {
            var metadata = localStorage.GetMetadata();
            var processedBlobs = new List<string>();

            BlobContinuationToken token = null;
            do
            {
                var segment = await container.ListBlobsSegmentedAsync(null, true, BlobListingDetails.None, null, token, null, null);
                token = segment.ContinuationToken;

                var result = await ProcessItemsAsync(segment.Results, metadata, log);
                processedBlobs.AddRange(result);
            } while (token != null);

            var missingPosts = (from p in metadata.Keys
                                where processedBlobs.All(item => item != p)
                                select p).ToList();
            missingPosts.ForEach(post =>
                                 {
                                     localStorage.DeletePost(post);
                                     metadata.Remove(post);
                                 });

            localStorage.SaveMetadata(metadata);
        }

        private async Task<List<string>> ProcessItemsAsync(IEnumerable<IListBlobItem> items, 
                                                           IDictionary<string, string> metadata, 
                                                           TextWriter log)
        {
            var result = new List<string>();

            foreach (var item in items)
            {
                var blob = item as CloudBlockBlob;
                if (blob == null)
                {
                    await log.WriteLineAsync(string.Format("'{0}' is not a block blob.", item.Uri));
                    continue;
                }

                var blobName = GetBlobName(item);
                if (metadata.ContainsKey(blobName))
                {
                    if (!metadata[blobName].Equals(blob.Properties.ETag, StringComparison.OrdinalIgnoreCase))
                    {
                        await DownloadPostAsync(blobName, blob);
                        metadata[blobName] = blob.Properties.ETag;
                    }
                }
                else
                {
                    await DownloadPostAsync(blobName, blob);
                    metadata.Add(blobName, blob.Properties.ETag);
                }

                result.Add(blobName);
            }

            return result;
        }

        private async Task DownloadPostAsync(string name, ICloudBlob blob)
        {
            using (var stream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(stream);
                localStorage.SavePost(name, stream.ToArray());
            }
        }

        private static string GetBlobName(IListBlobItem item)
        {
            var name = item.Uri.ToString().Replace(item.Container.Uri.ToString(), string.Empty);
            return name.Length > 1 && name.StartsWith("/") ? name.Substring(1) : name;
        }
    }
}
