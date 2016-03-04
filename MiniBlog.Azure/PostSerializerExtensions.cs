using System.IO;
using MiniBlog.Contracts;
using MiniBlog.Contracts.Model;

namespace MiniBlog.Azure
{
    internal static class PostSerializerExtensions
    {
        public static byte[] SerializeAsByteArray(this IPostSerializer serializer, Post post)
        {
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(post, stream);
                return stream.ToArray();
            }
        }
    }
}