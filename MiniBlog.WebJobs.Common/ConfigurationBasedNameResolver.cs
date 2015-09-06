using Microsoft.Azure.WebJobs;
using MiniBlog.Contracts.Framework;

namespace MiniBlog.WebJobs.Common
{
    internal class ConfigurationBasedNameResolver : INameResolver
    {
        private readonly IConfiguration configuration;

        public ConfigurationBasedNameResolver(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string Resolve(string name)
        {
            return configuration.Find(name);
        }
    }
}