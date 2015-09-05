using System.IO;
using MiniBlog.Contracts.Framework;
using MiniBlog.Contracts.Model;

namespace MiniBlog.Azure.Commands
{
    internal class DeletePostLocallyCommand : ICommand<string>
    {
        private readonly Post post;

        public DeletePostLocallyCommand(Post post)
        {
            this.post = post;
        }

        public void Apply(string model)
        {
            var path = Path.Combine(model, post.GetFileName());
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}