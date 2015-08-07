using System.Configuration;
using MiniBlog.Contracts;

internal class Configuration : IConfiguration
{
    public string Find(string key)
    {
        return ConfigurationManager.AppSettings[key];
    }
}