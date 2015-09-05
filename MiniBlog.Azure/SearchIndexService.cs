using MiniBlog.Azure.Commands;
using MiniBlog.Contracts;
using MiniBlog.Contracts.Model;

namespace MiniBlog.Azure
{
    internal class SearchIndexService : ISearchIndexService
    {
        private readonly ICloudQueueFactory queueFactory;

        public SearchIndexService(ICloudQueueFactory queueFactory)
        {
            this.queueFactory = queueFactory;
        }

        public void Index(Post post)
        {
            var command = new SendSearchIndexMessageCommand(post.GetFileName());
            command.Apply(queueFactory.Create(Constants.IndexQueueKey));
        }

        public void Delete(string postId)
        {
            var command = new SendSearchIndexMessageCommand(postId);
            command.Apply(queueFactory.Create(Constants.DeleteQueueKey));
        }
    }
}