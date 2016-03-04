using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using MiniBlog.Contracts;
using BlogPost = MiniBlog.Contracts.Model.Post;

namespace Data
{
    internal class LocalPostStorage : IPostStorage
    {
        private static readonly string Folder = HostingEnvironment.MapPath("~/posts/");
        private readonly IPostSerializer postSerializer;

        public LocalPostStorage(IPostSerializer postSerializer)
        {
            this.postSerializer = postSerializer;
        }

        public List<BlogPost> GetAllPosts()
        {
            return LoadPosts();
        }
        
        public void Save(BlogPost post)
        {
            post.LastModified = DateTime.UtcNow;
            using (var stream = File.OpenWrite(GetFileName(post)))
            {
                postSerializer.Serialize(post, stream);
            }
        }

        public void Delete(BlogPost post)
        {
            var file = GetFileName(post);
            File.Delete(file);
        }

        private List<BlogPost> LoadPosts()
        {
            if (!Directory.Exists(Folder))
                Directory.CreateDirectory(Folder);

            var list = new List<BlogPost>();
            
            foreach (var file in Directory.EnumerateFiles(Folder, string.Format("*{0}", Constants.PostFileExtension), SearchOption.TopDirectoryOnly))
            {
                using (var stream = File.OpenRead(file))
                {
                    var post = postSerializer.Deserialize(stream, Path.GetFileNameWithoutExtension(file));
                    list.Add(post);
                }
            }

            return list;
        }

        private static string GetFileName(BlogPost post)
        {
            return Path.Combine(Folder, post.GetFileName());
        }
    }
}