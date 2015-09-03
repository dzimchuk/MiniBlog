using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MiniBlog.Contracts;
using Newtonsoft.Json;

namespace MiniBlog.PostSync.Storage
{
    internal class FileBasedChangeNotifier : IChangeNotifier
    {
        private readonly ILocalPathProvider localPathProvider;
        private readonly List<string> changes = new List<string>();

        public FileBasedChangeNotifier(ILocalPathProvider localPathProvider)
        {
            this.localPathProvider = localPathProvider;
        }

        public void TrachChange(string fileName)
        {
            changes.Add(fileName);
        }

        public Task NotifyAsync()
        {
            if (changes.Any())
            {
                var path = Path.Combine(localPathProvider.GetAppDataPath(), Constants.NotificationFileName);
                File.WriteAllText(path, JsonConvert.SerializeObject(changes));
            }

            return Task.FromResult(0);
        }
    }
}