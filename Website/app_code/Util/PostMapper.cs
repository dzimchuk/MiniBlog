using AutoMapper;
using Data;

namespace Util
{
    internal class PostMapper : IPostMapper
    {
        static PostMapper()
        {
            Mapper.Initialize(configuration =>
                              {
                                  configuration.CreateMap<MiniBlog.Contracts.Model.Comment, Comment>()
                                               .ForMember(dest => dest.ID, config => config.MapFrom(source => source.Id))
                                               .ReverseMap()
                                               .ForMember(dest => dest.Id, config => config.MapFrom(source => source.ID));
                                  configuration.CreateMap<MiniBlog.Contracts.Model.Post, Post>()
                                               .ForMember(dest => dest.ID, config => config.MapFrom(source => source.Id))
                                               .ForMember(dest => dest.AbsoluteUrl, config => config.Ignore())
                                               .ForMember(dest => dest.Url, config => config.Ignore())
                                               .ReverseMap()
                                               .ForMember(dest => dest.Id, config => config.MapFrom(source => source.ID));
                              });
            
            Mapper.AssertConfigurationIsValid();
        }

        public Post MapFrom(MiniBlog.Contracts.Model.Post post)
        {
            return Mapper.Map<Post>(post);
        }

        public MiniBlog.Contracts.Model.Post MapFrom(Post post)
        {
            return Mapper.Map<MiniBlog.Contracts.Model.Post>(post);
        }
    }
}