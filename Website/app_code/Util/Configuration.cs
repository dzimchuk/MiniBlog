using System;
using System.Configuration;
using MiniBlog.Contracts;

namespace Util
{
    internal class Configuration : IConfiguration
    {
        public string Find(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}