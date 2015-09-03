using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using System.Xml.Linq;
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
            var doc = postSerializer.Serialize(post);

            var file = GetFileName(post);
            doc.Save(file);
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
            
            foreach (var file in Directory.EnumerateFiles(Folder, "*.xml", SearchOption.TopDirectoryOnly))
            {
                var doc = XElement.Load(file);
                var post = postSerializer.Deserialize(doc, Path.GetFileNameWithoutExtension(file));

                list.Add(post);
            }

            return list;
        }

        private static string GetFileName(BlogPost post)
        {
            return Path.Combine(Folder, post.GetFileName());
        }
    }
}