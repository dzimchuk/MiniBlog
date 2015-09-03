using System.Collections.Generic;
using System.IO;
using MiniBlog.Contracts;
using Newtonsoft.Json;

namespace MiniBlog.PostSync.Storage
{
    internal class LocalStorage : ILocalStorage
    {
        private const string MetadataFile = "metadata.json";
        private readonly ILocalPathProvider localPathProvider;

        public LocalStorage(ILocalPathProvider localPathProvider)
        {
            this.localPathProvider = localPathProvider;
        }

        public Dictionary<string, string> GetMetadata()
        {
            var path = Path.Combine(GetPostDirectory(), MetadataFile);
            return !File.Exists(path)
                ? new Dictionary<string, string>()
                : JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path));
        }

        public void SaveMetadata(Dictionary<string, string> metadata)
        {
            var path = Path.Combine(GetPostDirectory(), MetadataFile);
            File.WriteAllText(path, JsonConvert.SerializeObject(metadata));
        }

        public void SavePost(string name, byte[] content)
        {
            var path = Path.Combine(GetPostDirectory(), name);
            File.WriteAllBytes(path, content);
        }

        public void DeletePost(string name)
        {
            var path = Path.Combine(GetPostDirectory(), name);
            if (File.Exists(path))
                File.Delete(path);
        }

        private string GetPostDirectory()
        {
            var directory = Path.Combine(localPathProvider.GetAppDataPath(), Constants.PostDirectory);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return directory;
        }
    }
}