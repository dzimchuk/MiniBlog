using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using MiniBlog.Contracts;
using MiniBlog.Contracts.Model.Search;

public static class SearchFacade
{
    private static readonly ISearchService Service = ServiceLocator.Current.GetInstance<ISearchService>();

    public static IList<SearchResult> Search(string searchText)
    {
        return Service.Search(searchText);
    }

    public static IList<SuggestResult> Suggest(string searchText)
    {
        return Service.Suggest(searchText);
    }

    public static void Index(Post post)
    {
        
    }

    public static void Delete(string postId)
    {
        
    }
}