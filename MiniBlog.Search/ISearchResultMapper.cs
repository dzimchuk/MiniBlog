using System.Collections.Generic;
using MiniBlog.Contracts.Model.Search;

namespace MiniBlog.Search
{
    public interface ISearchResultMapper
    {
        IList<SearchResult> MapFrom(IList<Microsoft.Azure.Search.Models.SearchResult> results);
        IList<SuggestResult> MapFrom(IList<Microsoft.Azure.Search.Models.SuggestResult> results);
    }
}