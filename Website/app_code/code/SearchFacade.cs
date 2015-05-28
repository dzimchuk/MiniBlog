using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

internal static class SearchFacade
{
    public static IList<SearchResult> Search(string searchText)
    {
        var parameters = new SearchParameters
        {
            SearchMode = SearchMode.All,
            HighlightFields = new List<string> { "title", "content" }
        };

        var result = IndexClient.Documents.Search(searchText, parameters);
        return result.StatusCode != HttpStatusCode.OK ? null : result.Results;
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
}