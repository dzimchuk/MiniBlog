using System.IO;
using MiniBlog.Contracts;
using MiniBlog.Contracts.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MiniBlog.Services
{
    internal class JsonPostSerializer : IPostSerializer
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
                                                                            {
                                                                                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                                                                                NullValueHandling = NullValueHandling.Ignore,
                                                                                DateParseHandling = DateParseHandling.DateTimeOffset
                                                                            };

        public void Serialize(Post post, Stream stream)
        {
            var json = JsonConvert.SerializeObject(post, SerializerSettings);
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(json);
            }
        }

        public Post Deserialize(Stream stream, string postId)
        {
            using (var reader = new StreamReader(stream))
            {
                var post = JsonConvert.DeserializeObject<Post>(reader.ReadToEnd(), SerializerSettings);
                post.Id = postId;

                return post;
            }
        }
    }
}