using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Azure.WebJobs;
using MiniBlog.Contracts;
using MiniBlog.Search.Extensions;

namespace MiniBlog.PostIndexer
{
    public class Functions
    {
        private readonly ISearchIndexClient searchIndexClient;
        private readonly IPostSerializer postSerializer;

        public Functions(ISearchIndexClient searchIndexClient, IPostSerializer postSerializer)
        {
            this.searchIndexClient = searchIndexClient;
            this.postSerializer = postSerializer;
        }

        public async Task IndexPostAsync([QueueTrigger("%search:indexQueue%")] string message, 
                                         [Blob("%blog:postContainer%/{queueTrigger}", FileAccess.Read)] Stream stream,
                                         TextWriter log)
        {
            await log.WriteLineAsync($"Indexing post {message}...");
            
            var post = postSerializer.Deserialize(stream, Path.GetFileNameWithoutExtension(message));

            var action = IndexAction.MergeOrUpload(post.ToDocument());
            await searchIndexClient.IndexWithRetryAsync(action);

            await log.WriteLineAsync("Done.");
        }

        public async Task DeletePostAsync([QueueTrigger("%search:deleteQueue%")] string message,
                                          TextWriter log)
        {
            await log.WriteLineAsync($"Deleting post {message} from index...");

            var action = IndexAction.Delete(new Document { { "Id", message} });
            await searchIndexClient.IndexWithRetryAsync(action);

            await log.WriteLineAsync("Done.");
        }
    }
}
