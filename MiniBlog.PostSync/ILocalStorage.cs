using System.Collections.Generic;

namespace MiniBlog.PostSync
{
    public interface ILocalStorage
    {
        Dictionary<string, string> GetMetadata();
        void SaveMetadata(Dictionary<string, string> metadata);
        void SavePost(string name, byte[] content);
        void DeletePost(string name);
    }
}