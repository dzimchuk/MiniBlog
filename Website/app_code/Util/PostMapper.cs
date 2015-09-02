using AutoMapper;
using Data;

namespace Util
{
    internal class PostMapper : IPostMapper
    {
        static PostMapper()
        {
            Mapper.CreateMap<MiniBlog.Contracts.Model.Comment, Comment>().ReverseMap();
            Mapper.CreateMap<MiniBlog.Contracts.Model.Post, Post>().ReverseMap();
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