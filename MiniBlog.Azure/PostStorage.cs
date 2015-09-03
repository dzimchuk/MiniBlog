using System.Collections.Generic;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MiniBlog.Azure.Commands;
using MiniBlog.Azure.Queries;
using MiniBlog.Contracts;
using MiniBlog.Contracts.Model;

namespace MiniBlog.Azure
{
    internal class PostStorage : IPostStorage
    {
        private readonly ILocalPathProvider localPathProvider;
        private readonly IPostSerializer postSerializer;
        private readonly IConfiguration configuration;

        public PostStorage(ILocalPathProvider localPathProvider, IPostSerializer postSerializer, IConfiguration configuration)
        {
            this.localPathProvider = localPathProvider;
            this.postSerializer = postSerializer;
            this.configuration = configuration;
        }

        public List<Post> GetAllPosts()
        {
            var query = new ListLocalPostsQuery(postSerializer);
            return query.Execute(GetPostDiretory());
        }

        public void Save(Post post)
        {
            var persistPostBlobCommand = new PersistPostBlobCommand(post, postSerializer);
            persistPostBlobCommand.Apply(GetCloudContainer());

            var persistPostLocallyCommand = new PersistPostLocallyCommand(post, postSerializer);
            persistPostLocallyCommand.Apply(GetPostDiretory());
        }

        public void Delete(Post post)
        {
            var deletePostBlobCommand = new DeletePostBlobCommand(post);
            deletePostBlobCommand.Apply(GetCloudContainer());

            var deletePostLocallyCommand = new DeletePostLocallyCommand(post);
            deletePostLocallyCommand.Apply(GetPostDiretory());
        }

        private string GetPostDiretory()
        {
            var diretory = Path.Combine(localPathProvider.GetAppDataPath(), Constants.PostDirectory);
            if (!Directory.Exists(diretory))
                Directory.CreateDirectory(diretory);

            return diretory;
        }

        private CloudBlobContainer GetCloudContainer()
        {
            var storageAccount = CloudStorageAccount.Parse(configuration.Find("blog:contentStorage"));
            var client = storageAccount.CreateCloudBlobClient();
            return client.GetContainerReference(configuration.Find("blog:postContainer"));
        }
    }
}