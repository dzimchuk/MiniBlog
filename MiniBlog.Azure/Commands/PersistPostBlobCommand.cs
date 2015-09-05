using Microsoft.WindowsAzure.Storage.Blob;
using MiniBlog.Contracts;
using MiniBlog.Contracts.Framework;
using MiniBlog.Contracts.Model;

namespace MiniBlog.Azure.Commands
{
    internal class PersistPostBlobCommand : ICommand<CloudBlobContainer>
    {
        private readonly Post post;
        private readonly IPostSerializer postSerializer;

        public PersistPostBlobCommand(Post post, IPostSerializer postSerializer)
        {
            this.post = post;
            this.postSerializer = postSerializer;
        }

        public void Apply(CloudBlobContainer model)
        {
            var content = postSerializer.SerializeAsByteArray(post);

            var blob = model.GetBlockBlobReference(post.GetFileName());
            blob.Properties.ContentType = "text/xml";

            blob.UploadFromByteArray(content, 0, content.Length);
        }
    }
}