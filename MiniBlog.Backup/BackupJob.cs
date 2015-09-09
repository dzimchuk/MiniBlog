using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MiniBlog.Contracts.Framework;

namespace MiniBlog.Backup
{
    public class BackupJob
    {
        private readonly IConfiguration configuration;
        private TextWriter log;
        private ZipArchive zipArchive;
        private string folder;

        public BackupJob(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [NoAutomaticTrigger]
        public async Task BackupBlogAsync(CloudStorageAccount storageAccount, TextWriter log)
        {
            this.log = log;

            await log.WriteLineAsync("Backup started");
            var client = storageAccount.CreateCloudBlobClient();

            using (var stream = new MemoryStream())
            {
                using (zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, true))
                {
                    folder = "posts";
                    var container = client.GetContainerReference(configuration.Find("blog:postContainer"));
                    await BackupContainer(container);

                    folder = "files";
                    container = client.GetContainerReference(configuration.Find("blog:contentContainer"));
                    await BackupContainer(container);
                }

                await PersistArchiveAsync(stream.ToArray());
            }

            await log.WriteLineAsync("Backup finished");
        }

        private async Task BackupContainer(CloudBlobContainer container)
        {
            await log.WriteLineAsync($"Container: {container.Name}");
            await ProcessDirectoryAsync(container, null);
        }

        private async Task ProcessDirectoryAsync(CloudBlobContainer container, string prefix)
        {
            BlobContinuationToken token = null;
            do
            {
                var segment = await container.ListBlobsSegmentedAsync(prefix, false, BlobListingDetails.None, null, token, null, null);
                token = segment.ContinuationToken;

                await ProcessItemsAsync(segment.Results);
            } while (token != null);
        }

        private async Task ProcessItemsAsync(IEnumerable<IListBlobItem> items)
        {
            foreach (var item in items)
            {
                if (item is CloudBlockBlob)
                {
                    await ProcessBlockBlobAsync((CloudBlockBlob)item);
                }
                else if (item is CloudBlobDirectory)
                {
                    await ProcessDirectoryAsync((CloudBlobDirectory)item);
                }
                else
                {
                    var message = $"Unsupported item type: {item.GetType()}";
                    await log.WriteLineAsync(message);

                    throw new Exception(message);
                }
            }
        }

        private Task ProcessDirectoryAsync(CloudBlobDirectory item)
        {
            return ProcessDirectoryAsync(item.Container, item.Prefix);
        }

        private async Task ProcessBlockBlobAsync(CloudBlockBlob item)
        {
            var entry = CreateEntry(item);
            using (var stream = entry.Open())
            {
                await item.DownloadToStreamAsync(stream);
            }

            await log.WriteLineAsync($"Added {item.Name}");
        }

        private ZipArchiveEntry CreateEntry(CloudBlockBlob item)
        {
            var entryName = string.IsNullOrEmpty(folder)
                ? item.Name
                : $"{folder}/{item.Name}";
            return zipArchive.CreateEntry(entryName);
        }

        private async Task PersistArchiveAsync(byte[] content)
        {
            var storageAccount = CloudStorageAccount.Parse(configuration.Find("blog:backupStorage"));
            var client = storageAccount.CreateCloudBlobClient();

            var container = client.GetContainerReference(configuration.Find("blog:backupContainer"));
            await container.CreateIfNotExistsAsync();

            var blobName = $"{DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH_mm_ss")}.zip";
            var blob = container.GetBlockBlobReference(blobName);

            await blob.UploadFromByteArrayAsync(content, 0, content.Length);
        }
    }
}
