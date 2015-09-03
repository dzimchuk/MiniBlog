using System.IO;
using System.Xml;
using MiniBlog.Contracts;
using MiniBlog.Contracts.Model;

namespace MiniBlog.Azure
{
    internal static class PostSerializerExtensions
    {
        public static byte[] SerializeAsByteArray(this IPostSerializer serializer, Post post)
        {
            var doc = serializer.Serialize(post);
            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream))
                {
                    doc.WriteTo(writer);
                }

                return stream.ToArray();
            }
        }
    }
}