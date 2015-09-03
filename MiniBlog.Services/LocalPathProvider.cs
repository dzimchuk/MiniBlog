using System;
using System.IO;
using MiniBlog.Contracts;

namespace MiniBlog.Services
{
    internal class LocalPathProvider : ILocalPathProvider
    {
        private const string AppData = "App_Data";
        private readonly IConfiguration configuration;

        public LocalPathProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetAppDataPath()
        {
#if DEBUG
            return Path.Combine(configuration.Find("WebRootPath"), AppData);
#else
            var webRoot = Environment.GetEnvironmentVariable("WEBROOT_PATH");
            if (string.IsNullOrWhiteSpace(webRoot))
                throw new Exception("WEBROOT_PATH not defined");

            return Path.Combine(webRoot, AppData);
#endif
        }
    }
}