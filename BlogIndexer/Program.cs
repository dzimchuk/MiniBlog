using System;
using System.Collections.Generic;
using System.Linq;
using BlogIndexer.Model;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using PowerArgs;

namespace BlogIndexer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var parsedArgs = Args.Parse<IndexerArgs>(args);
                IndexPosts(parsedArgs);
            }
            catch (ArgException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ArgUsage.GenerateUsageFromTemplate<IndexerArgs>());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void IndexPosts(IndexerArgs args)
        {
            System.Diagnostics.Debugger.Launch();

            var loader = new PostLoader(args.PostDirectory);
            var posts = loader.LoadPosts();

            Console.WriteLine("Posts to index: {0}", posts.Count);

            var client = new SearchServiceClient(args.SearchService, new SearchCredentials(args.Key));
            DeleteIndexIfExists(client, args.IndexName);
            CreateIndex(client, args.IndexName);
            UploadPosts(client.Indexes.GetClient(args.IndexName), posts);

            Console.WriteLine("Done.");
        }

        private static void DeleteIndexIfExists(ISearchServiceClient client, string indexName)
        {
            if (client.Indexes.Exists(indexName))
            {
                Console.WriteLine("Deleting index {0}...", indexName);
                client.Indexes.Delete(indexName);
            }
        }

        private static void CreateIndex(ISearchServiceClient client, string indexName)
        {
            Console.WriteLine("Creating index {0}...", indexName);

            var definition = new Index
                             {
                                 Name = indexName,
                                 Fields = new List<Field>
                                          {
                                              new Field("Id", DataType.String) { IsKey = true },
                                              new Field("Title", DataType.String, AnalyzerName.EnLucene) { IsSearchable = true },
                                              new Field("Content", DataType.String, AnalyzerName.EnLucene) { IsSearchable = true },
                                              new Field("Categories", DataType.Collection(DataType.String), AnalyzerName.EnLucene) { IsSearchable = true },
                                              new Field("IsPublished", DataType.Boolean) { IsFilterable = true, IsRetrievable = false },
                                              new Field("PubDate", DataType.DateTimeOffset) { IsFilterable = true, IsRetrievable = false },
                                          }
                             };

            client.Indexes.Create(definition);
        }

        private static void UploadPosts(ISearchIndexClient client, List<Post> posts)
        {
            Console.WriteLine("Uploading posts...");

            try
            {
                client.Documents.Index(IndexBatch.Create(posts.Select(IndexAction.Create)));
            }
            catch (IndexBatchException e)
            {
                Console.WriteLine(
                    "Failed to index some of the documents: {0}",
                    string.Join(", ", e.IndexResponse.Results.Where(r => !r.Succeeded).Select(r => r.Key)));
            }
        }
    }
}
