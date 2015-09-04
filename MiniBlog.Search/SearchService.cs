using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using MiniBlog.Contracts;
using SearchResult = MiniBlog.Contracts.Model.Search.SearchResult;
using SuggestResult = MiniBlog.Contracts.Model.Search.SuggestResult;

namespace MiniBlog.Search
{
    internal class SearchService : ISearchService
    {
        private readonly ISearchIndexClient indexClient;
        private readonly ISearchResultMapper mapper;

        public SearchService(ISearchIndexClient indexClient, ISearchResultMapper mapper)
        {
            this.indexClient = indexClient;
            this.mapper = mapper;
        }

        public IList<SearchResult> Search(string searchText)
        {
            var parameters = new SearchParameters
            {
                SearchMode = SearchMode.All,
                HighlightFields = new List<string> { "Content" },
                HighlightPreTag = "<b>",
                HighlightPostTag = "</b>",
                Filter = "IsPublished eq true"
            };

            var result = indexClient.Documents.Search(searchText, parameters);
            return result.StatusCode != HttpStatusCode.OK ? null : mapper.MapFrom(result.Results);
        }

        public IList<SuggestResult> Suggest(string searchText)
        {
            var parameters = new SuggestParameters
            {
                UseFuzzyMatching = true,
                HighlightPreTag = "<b>",
                HighlightPostTag = "</b>",
                Filter = "IsPublished eq true"
            };

            var result = indexClient.Documents.Suggest(searchText, "sg", parameters);
            return result.StatusCode != HttpStatusCode.OK ? null : mapper.MapFrom(result.Results);
        }
    }
}