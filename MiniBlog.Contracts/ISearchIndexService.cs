using MiniBlog.Contracts.Model;

namespace MiniBlog.Contracts
{
    public interface ISearchIndexService
    {
        void Index(Post post);
        void Delete(string postId);
    }
}