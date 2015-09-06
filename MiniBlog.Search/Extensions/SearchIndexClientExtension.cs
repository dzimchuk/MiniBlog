using System;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace MiniBlog.Search.Extensions
{
    public static class SearchIndexClientExtension
    {
        public static Task IndexWithRetryAsync(this ISearchIndexClient client, IndexAction action)
        {
            var retryPolicy = GetRetryPolicy();
            return retryPolicy.ExecuteAsync(() => client.Documents.IndexAsync(IndexBatch.Create(action)));
        }

        public static Task IndexWithRetryAsync(this ISearchIndexClient client, IndexAction[] actions)
        {
            var retryPolicy = GetRetryPolicy();
            return retryPolicy.ExecuteAsync(() => client.Documents.IndexAsync(IndexBatch.Create(actions)));
        }

        private static RetryPolicy GetRetryPolicy()
        {
            var retryStrategy = new Incremental(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
            return new RetryPolicy<SearchIndexErrorDetectionStrategy>(retryStrategy);
        }
    }
}