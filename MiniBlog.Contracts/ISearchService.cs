using System.Collections.Generic;
using MiniBlog.Contracts.Model.Search;

namespace MiniBlog.Contracts
{
    public interface ISearchService
    {
        IList<SearchResult> Search(string searchText);
        IList<SuggestResult> Suggest(string searchText);
    }
}