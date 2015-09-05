using System.Collections.Generic;
using AutoMapper;
using MiniBlog.Contracts.Model.Search;

namespace MiniBlog.Search.Mappers
{
    internal class SearchResultMapper : ISearchResultMapper
    {
        static SearchResultMapper()
        {
            Mapper.CreateMap<Microsoft.Azure.Search.Models.Document, Document>();
            Mapper.CreateMap<Microsoft.Azure.Search.Models.HitHighlights, HitHighlights>();
            Mapper.CreateMap<Microsoft.Azure.Search.Models.SearchResult, SearchResult>();
            Mapper.CreateMap<Microsoft.Azure.Search.Models.SuggestResult, SuggestResult>();

            Mapper.AssertConfigurationIsValid();
        }

        public IList<SearchResult> MapFrom(IList<Microsoft.Azure.Search.Models.SearchResult> results)
        {
            return Mapper.Map<IList<SearchResult>>(results);
        }

        public IList<SuggestResult> MapFrom(IList<Microsoft.Azure.Search.Models.SuggestResult> results)
        {
            return Mapper.Map<IList<SuggestResult>>(results);
        }
    }
}