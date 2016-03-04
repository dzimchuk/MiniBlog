using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using MiniBlog.Contracts;

namespace Data
{
    internal class CachingPostStorage : IStorageAdapter
    {
        private readonly IPostStorage storage;
        private readonly IPostMapper mapper;
        private readonly ILocalPathProvider localPathProvider;

        public CachingPostStorage(IPostStorage storage, IPostMapper mapper, ILocalPathProvider localPathProvider)
        {
            this.storage = storage;
            this.mapper = mapper;
            this.localPathProvider = localPathProvider;
        }

        public List<Post> GetAllPosts()
        {
            var posts = HttpRuntime.Cache["posts"] as List<Post>;
            if (posts == null)
            {
                posts = storage.GetAllPosts().Select(post => mapper.MapFrom(post)).ToList();
                SortAndCache(posts);
            }

            return posts;
        }
    
        public void Save(Post post)
        {
            storage.Save(mapper.MapFrom(post));

            var posts = GetAllPosts();
            if (posts.All(p => p.ID != post.ID))
            {
                posts.Insert(0, post);
                SortAndCache(posts);
            }
            else
            {
                Blog.ClearStartPageCache();
            }
        }

        public void Delete(Post post)
        {
            storage.Delete(mapper.MapFrom(post));

            var posts = GetAllPosts();
            var cachedPost = posts.FirstOrDefault(p => p.ID == post.ID);
            if (cachedPost != null)
            {
                posts.Remove(cachedPost);
            }

            Blog.ClearStartPageCache();
        }

        private void SortAndCache(List<Post> posts)
        {
            posts.Sort((p1, p2) => p2.PubDate.CompareTo(p1.PubDate));
            HttpRuntime.Cache.Insert("posts", posts, CreateCacheDependency());
        }

        private CacheDependency CreateCacheDependency()
        {
            var path = Path.Combine(localPathProvider.GetAppDataPath(), Constants.NotificationFileName);
            return new CacheDependency(path);
        }
    }
}