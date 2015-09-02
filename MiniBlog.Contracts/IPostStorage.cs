using System;
using System.Collections.Generic;
using MiniBlog.Contracts.Model;

namespace MiniBlog.Contracts
{
    public interface IPostStorage
    {
        List<Post> GetAllPosts();
        void Save(Post post);
        void Delete(Post post);
    }
}