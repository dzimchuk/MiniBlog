using System;
using System.Globalization;
using MiniBlog.Contracts;

namespace MiniBlog.Azure
{
    internal class CdnFileStorage : IFileStorage
    {
        private readonly IFileStorage storage;
        private readonly IConfiguration configuration;

        public CdnFileStorage(IFileStorage storage, IConfiguration configuration)
        {
            this.storage = storage;
            this.configuration = configuration;
        }

        public string Save(byte[] bytes, string extension)
        {
            var url = storage.Save(bytes, extension);

            var cdnUrl = configuration.Find("blog:contentCdnUrl");
            if (string.IsNullOrWhiteSpace(cdnUrl))
                return url;

            var containerName = configuration.Find("blog:contentContainer");

            var index = url.IndexOf(containerName, StringComparison.OrdinalIgnoreCase);
            return index > -1
                ? string.Format(CultureInfo.InvariantCulture, "{0}/{1}", cdnUrl, url.Substring(index))
                : url;
        }
    }
}