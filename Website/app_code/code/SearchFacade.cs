using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

internal static class SearchFacade
{
    public static IList<SearchResult> Search(string searchText)
    {
        var parameters = new SearchParameters
        {
            SearchMode = SearchMode.All,
            HighlightFields = new List<string> { "Title", "Content" },
            Filter = "IsPublished eq true"
        };

        var result = IndexClient.Documents.Search(searchText, parameters);
        return result.StatusCode != HttpStatusCode.OK ? null : result.Results;
    }

    public static void Index(Post post)
    {
        var document = new Document
                       {
                           { "Id", post.ID },
                           { "Title", post.Title },
                           { "Content", post.Content },
                           { "Categories", post.Categories },
                           { "IsPublished", post.IsPublished },
                           { "PubDate", post.PubDate }
                       };

        var action = new IndexAction(document);
        Execute(action);
    }

    public static void Delete(string postId)
    {
        var document = new Document
                       {
                           { "Id", postId }
                       };

        var action = new IndexAction(IndexActionType.Delete, document);
        Execute(action);
    }

    private static void Execute(IndexAction action)
    {
        var retryStrategy = new Incremental(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
        var retryPolicy = new RetryPolicy<SearchIndexErrorDetectionStrategy>(retryStrategy);

        retryPolicy.ExecuteAction(() => IndexClient.Documents.Index(IndexBatch.Create(action)));
    }

    private static ISearchIndexClient IndexClient
    {
        get
        {
            return new SearchIndexClient(ConfigurationManager.AppSettings["search:service"],
                                         ConfigurationManager.AppSettings["search:index"],
                                         new SearchCredentials(ConfigurationManager.AppSettings["search:key"]));
        }
    }

    private class SearchIndexErrorDetectionStrategy : ITransientErrorDetectionStrategy
    {
        public bool IsTransient(Exception ex)
        {
            return ex is IndexBatchException;
        }
    }
}