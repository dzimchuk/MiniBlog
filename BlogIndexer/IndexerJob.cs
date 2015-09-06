using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MiniBlog.Contracts;
using MiniBlog.Contracts.Framework;
using MiniBlog.Contracts.Model;
using MiniBlog.Search.Extensions;

namespace BlogIndexer
{
    public class IndexerJob
    {
        private readonly IConfiguration configuration;
        private readonly ISearchServiceClient searchServiceClient;
        private readonly ISearchIndexClient searchIndexClient;
        private readonly IPostSerializer postSerializer;

        public IndexerJob(IConfiguration configuration, 
                          ISearchServiceClient searchServiceClient, 
                          ISearchIndexClient searchIndexClient, 
                          IPostSerializer postSerializer)
        {
            this.configuration = configuration;
            this.searchServiceClient = searchServiceClient;
            this.searchIndexClient = searchIndexClient;
            this.postSerializer = postSerializer;
        }

        [NoAutomaticTrigger]
        public async Task IndexAsync(CloudStorageAccount storageAccount, TextWriter log)
        {
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(configuration.Find("blog:postContainer"));

            await log.WriteLineAsync("BlogIndexer started");
            await IndexAsync(container, log);
            await log.WriteLineAsync("BlogIndexer finished");
        }

        private async Task IndexAsync(CloudBlobContainer container, TextWriter log)
        {
            var indexName = configuration.Find("search:index");
            await DeleteIndexAsync(indexName, log);
            await CreateIndexAsync(indexName, log);

            BlobContinuationToken token = null;
            do
            {
                var segment = await container.ListBlobsSegmentedAsync(null, true, BlobListingDetails.None, null, token, null, null);
                token = segment.ContinuationToken;

                await ProcessItemsAsync(segment.Results, log);
            } while (token != null);
        }

        
        private async Task ProcessItemsAsync(IEnumerable<IListBlobItem> items, TextWriter log)
        {
            var posts = new List<Post>();

            foreach (var item in items)
            {
                var blob = item as CloudBlockBlob;
                if (blob == null)
                {
                    await log.WriteLineAsync($"'{item.Uri}' is not a block blob.");
                    continue;
                }

                var doc = await DownloadAsync(blob);
                var post = postSerializer.Deserialize(doc, Path.GetFileNameWithoutExtension(GetBlobName(item)));

                posts.Add(post);
            }

            await UploadPostsAsync(posts, log);
        }

        private static async Task<XElement> DownloadAsync(CloudBlockBlob blob)
        {
            XElement doc;
            using (var stream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(stream);
                stream.Position = 0;

                doc = XElement.Load(stream);
            }
            return doc;
        }

        private async Task DeleteIndexAsync(string indexName, TextWriter log)
        {
            if (await searchServiceClient.Indexes.ExistsAsync(indexName))
            {
                await log.WriteLineAsync($"Deleting index {indexName}...");
                await searchServiceClient.Indexes.DeleteAsync(indexName);
            }
        }

        private async Task CreateIndexAsync(string indexName, TextWriter log)
        {
            await log.WriteLineAsync($"Creating index {indexName}...");

            var suggester = new Suggester
            {
                Name = "sg",
                SearchMode = SuggesterSearchMode.AnalyzingInfixMatching,
                SourceFields = new List<string> { "Title", "Categories" }
            };

            var definition = new Index
            {
                Name = indexName,
                Fields = new List<Field>
                                          {
                                              new Field("Id", DataType.String) { IsKey = true },
                                              new Field("Title", DataType.String) { IsSearchable = true, IsRetrievable = false },
                                              new Field("Content", DataType.String, AnalyzerName.EnLucene) { IsSearchable = true, IsRetrievable = false },
                                              new Field("Categories", DataType.Collection(DataType.String)) { IsSearchable = true, IsRetrievable = false },
                                              new Field("IsPublished", DataType.Boolean) { IsFilterable = true, IsRetrievable = false },
                                              new Field("PubDate", DataType.DateTimeOffset) { IsFilterable = true, IsRetrievable = false },
                                          },
                Suggesters = new List<Suggester> { suggester }
            };

            await searchServiceClient.Indexes.CreateAsync(definition);
        }

        private async Task UploadPostsAsync(IEnumerable<Post> posts, TextWriter log)
        {
            await log.WriteLineAsync("Uploading posts...");

            var actions = posts.Select(post => new IndexAction(post.ToDocument())).ToArray();
            try
            {
                await searchIndexClient.IndexWithRetryAsync(actions);
            }
            catch (IndexBatchException e)
            {
                await log.WriteLineAsync(
                    string.Format("Failed to index some of the documents: {0}",
                    string.Join(", ", e.IndexResponse.Results.Where(r => !r.Succeeded).Select(r => r.Key))));
            }
        }

        private static string GetBlobName(IListBlobItem item)
        {
            var name = item.Uri.ToString().Replace(item.Container.Uri.ToString(), string.Empty);
            return name.Length > 1 && name.StartsWith("/") ? name.Substring(1) : name;
        }
    }
}