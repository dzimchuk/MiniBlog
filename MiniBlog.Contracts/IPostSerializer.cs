using System.IO;
using MiniBlog.Contracts.Model;

namespace MiniBlog.Contracts
{
    public interface IPostSerializer
    {
        void Serialize(Post post, Stream stream);
        Post Deserialize(Stream stream, string postId);
    }
}