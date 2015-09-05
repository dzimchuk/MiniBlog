using System.Collections.Generic;
using Data;
using Microsoft.Practices.ServiceLocation;
using MiniBlog.Contracts;
using MiniBlog.Contracts.Model.Search;

public static class SearchFacade
{
    private static readonly ISearchService SearchService = ServiceLocator.Current.GetInstance<ISearchService>();
    private static readonly IPostMapper PostMapper = ServiceLocator.Current.GetInstance<IPostMapper>();
    private static readonly ISearchIndexService SearchIndexService = ServiceLocator.Current.GetInstance<ISearchIndexService>();

    public static IList<SearchResult> Search(string searchText)
    {
        return SearchService.Search(searchText);
    }

    public static void Index(Post post)
    {
        SearchIndexService.Index(PostMapper.MapFrom(post));
    }

    public static void Delete(string postId)
    {
        SearchIndexService.Delete(postId);
    }
}