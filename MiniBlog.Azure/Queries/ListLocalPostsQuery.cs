using System.Collections.Generic;
using System.IO;
using MiniBlog.Contracts;
using MiniBlog.Contracts.Framework;
using MiniBlog.Contracts.Model;

namespace MiniBlog.Azure.Queries
{
    internal class ListLocalPostsQuery : IQuery<string, List<Post>>
    {
        private readonly IPostSerializer postSerializer;

        public ListLocalPostsQuery(IPostSerializer postSerializer)
        {
            this.postSerializer = postSerializer;
        }

        public List<Post> Execute(string model)
        {
            var list = new List<Post>();

            foreach (var file in Directory.EnumerateFiles(model, $"*{Constants.PostFileExtension}", SearchOption.TopDirectoryOnly))
            {
                using (var stream = File.OpenRead(file))
                {
                    var post = postSerializer.Deserialize(stream, Path.GetFileNameWithoutExtension(file));
                    list.Add(post);
                }
            }

            return list;
        }
    }
}