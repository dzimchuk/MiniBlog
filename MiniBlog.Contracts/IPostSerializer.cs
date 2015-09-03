using System.Xml.Linq;
using MiniBlog.Contracts.Model;

namespace MiniBlog.Contracts
{
    public interface IPostSerializer
    {
        XDocument Serialize(Post post);
        Post Deserialize(XElement doc, string postId);
    }
}