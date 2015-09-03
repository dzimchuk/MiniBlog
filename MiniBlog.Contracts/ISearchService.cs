using System.Collections.Generic;
using Microsoft.Azure.Search.Models;

namespace MiniBlog.Contracts
{
    public interface ISearchService
    {
        IList<SearchResult> Search(string searchText);
        IList<SuggestResult> Suggest(string searchText);
    }
}