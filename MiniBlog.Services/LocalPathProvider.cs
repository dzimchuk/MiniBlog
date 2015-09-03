using System;
using System.IO;
using MiniBlog.Contracts;

namespace MiniBlog.Services
{
    internal class LocalPathProvider : ILocalPathProvider
    {
        private const string AppData = "App_Data";

        public string GetAppDataPath()
        {
#if DEBUG
            return Path.Combine(@"d:\dev\.github\MiniBlog\Website\", AppData);
#else
            var webRoot = Environment.GetEnvironmentVariable("WEBROOT_PATH");
            if (string.IsNullOrWhiteSpace(webRoot))
                throw new Exception("WEBROOT_PATH not defined");

            return Path.Combine(webRoot, AppData);
#endif
        }
    }
}