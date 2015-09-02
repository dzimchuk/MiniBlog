using System;

namespace Data
{
    public interface IPostMapper
    {
        Post MapFrom(MiniBlog.Contracts.Model.Post post);
        MiniBlog.Contracts.Model.Post MapFrom(Post post);
    }
}