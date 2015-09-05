using LightInject;
using Microsoft.Azure.Search;
using MiniBlog.Contracts;
using MiniBlog.Search.Mappers;

namespace MiniBlog.Search.Composition
{
    public class CompositionModule : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<ISearchResultMapper, SearchResultMapper>();
            serviceRegistry.Register<ISearchIndexClient>(factory => CreateSearchIndexClient(factory), "searchOnlyClient");
            serviceRegistry.Register<ISearchService>(factory => new SearchService(factory.GetInstance<ISearchIndexClient>("searchOnlyClient"),
                factory.GetInstance<ISearchResultMapper>()));
        }

        private ISearchIndexClient CreateSearchIndexClient(IServiceFactory factory)
        {
            var configuration = factory.GetInstance<IConfiguration>();
            return new SearchIndexClient(configuration.Find("search:service"), 
                configuration.Find("search:index"), 
                new SearchCredentials(configuration.Find("search:key")));
        }
    }
}