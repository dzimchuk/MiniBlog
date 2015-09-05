using System.IO;
using MiniBlog.Contracts;
using MiniBlog.Contracts.Framework;
using MiniBlog.Contracts.Model;

namespace MiniBlog.Azure.Commands
{
    internal class PersistPostLocallyCommand : ICommand<string>
    {
        private readonly Post post;
        private readonly IPostSerializer postSerializer;

        public PersistPostLocallyCommand(Post post, IPostSerializer postSerializer)
        {
            this.post = post;
            this.postSerializer = postSerializer;
        }

        public void Apply(string model)
        {
            var content = postSerializer.SerializeAsByteArray(post);
            var path = Path.Combine(model, post.GetFileName());

            File.WriteAllBytes(path, content);
        }
    }
}