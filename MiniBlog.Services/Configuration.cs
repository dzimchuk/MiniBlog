using System;
using System.Configuration;
using MiniBlog.Contracts.Framework;

namespace MiniBlog.Services
{
    internal class Configuration : IConfiguration
    {
        public string Find(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}