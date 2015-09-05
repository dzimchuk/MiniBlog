using LightInject;
using Microsoft.Azure.Search;
using MiniBlog.Contracts;
using MiniBlog.Contracts.Framework;
using MiniBlog.Search.Mappers;

namespace MiniBlog.Search.Composition
{
    public class CompositionModule : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<ISearchResultMapper, SearchResultMapper>();

            serviceRegistry.Register(factory => CreateSearchIndexClient(factory, "search:adminKey"));
            serviceRegistry.Register(factory => CreateSearchServiceClient(factory));

            serviceRegistry.Register(factory => CreateSearchIndexClient(factory, "search:key"), "searchOnlyClient");
            serviceRegistry.Register<ISearchService>(factory => new SearchService(factory.GetInstance<ISearchIndexClient>("searchOnlyClient"),
                factory.GetInstance<ISearchResultMapper>()));
        }

        private static ISearchIndexClient CreateSearchIndexClient(IServiceFactory factory, string apiKey)
        {
            var configuration = factory.GetInstance<IConfiguration>();
            return new SearchIndexClient(configuration.Find("search:service"),
                configuration.Find("search:index"),
                new SearchCredentials(configuration.Find(apiKey)));
        }

        private static ISearchServiceClient CreateSearchServiceClient(IServiceFactory factory)
        {
            var configuration = factory.GetInstance<IConfiguration>();
            return new SearchServiceClient(configuration.Find("search:service"),
                new SearchCredentials(configuration.Find("search:adminKey")));
        }
    }
}