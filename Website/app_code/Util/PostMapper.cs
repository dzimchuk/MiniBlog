using AutoMapper;
using Data;

namespace Util
{
    internal class PostMapper : IPostMapper
    {
        private readonly IMapper mapper;
        public PostMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
                                                        {
                                                            cfg.CreateMap<MiniBlog.Contracts.Model.Comment, Comment>()
                                                               .ForMember(dest => dest.ID, config => config.MapFrom(source => source.Id))
                                                               .ReverseMap()
                                                               .ForMember(dest => dest.Id, config => config.MapFrom(source => source.ID));

                                                            cfg.CreateMap<MiniBlog.Contracts.Model.Post, Post>()
                                                               .ForMember(dest => dest.ID, config => config.MapFrom(source => source.Id))
                                                               .ForMember(dest => dest.AbsoluteUrl, config => config.Ignore())
                                                               .ForMember(dest => dest.Url, config => config.Ignore())
                                                               .ReverseMap()
                                                               .ForMember(dest => dest.Id, config => config.MapFrom(source => source.ID));
                                                        });

            configuration.AssertConfigurationIsValid();
            mapper = configuration.CreateMapper();
        }

        public Post MapFrom(MiniBlog.Contracts.Model.Post post)
        {
            return mapper.Map<Post>(post);
        }

        public MiniBlog.Contracts.Model.Post MapFrom(Post post)
        {
            return mapper.Map<MiniBlog.Contracts.Model.Post>(post);
        }
    }
}