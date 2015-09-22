using System.Collections.Generic;
using MiniBlog.Azure.Queries;
using MiniBlog.Contracts;

namespace MiniBlog.Azure
{
    internal class OptimizedImageMapProvider : IOptimizedImageMapProvider
    {
        private readonly ICloudTableFactory tableFactory;

        public OptimizedImageMapProvider(ICloudTableFactory tableFactory)
        {
            this.tableFactory = tableFactory;
        }

        public Dictionary<string, string> GetMap()
        {
            var query = new GetOptimizedImageMapQuery();
            return query.Execute(tableFactory.Create(Constants.OptimizedImageMap));
        }
    }
}