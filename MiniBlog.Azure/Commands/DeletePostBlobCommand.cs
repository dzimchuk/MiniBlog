using Microsoft.WindowsAzure.Storage.Blob;
using MiniBlog.Contracts;
using MiniBlog.Contracts.Model;

namespace MiniBlog.Azure.Commands
{
    internal class DeletePostBlobCommand : ICommand<CloudBlobContainer>
    {
        private readonly Post post;

        public DeletePostBlobCommand(Post post)
        {
            this.post = post;
        }

        public void Apply(CloudBlobContainer model)
        {
            var blob = model.GetBlockBlobReference(post.GetFileName());
            blob.DeleteIfExists();
        }
    }
}