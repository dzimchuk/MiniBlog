using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;
using MiniBlog.Contracts.Framework;

namespace MiniBlog.Azure.Queries
{
    internal class GetOptimizedImageMapQuery : IQuery<CloudTable, Dictionary<string, string>>
    {
        private const string OriginalImagePath = "OriginalImagePath";
        private const string OptimizedImagePath = "OptimizedImagePath";

        public Dictionary<string, string> Execute(CloudTable model)
        {
            var result = new Dictionary<string, string>();
            var query = new TableQuery { FilterString = "PartitionKey eq 'Image'" };

            ReadTable(model, query, result);

            return result;
        }

        private static void ReadTable(CloudTable model, TableQuery query, Dictionary<string, string> result)
        {
            TableContinuationToken token = null;
            do
            {
                var segment = model.ExecuteQuerySegmented(query, token);
                segment.Results?.ForEach(item => AddItem(item, result));
                token = segment.ContinuationToken;
            } while (token != null);
        }

        private static void AddItem(DynamicTableEntity item, Dictionary<string, string> dictionary)
        {
            if (!item.Properties.ContainsKey(OriginalImagePath) || !item.Properties.ContainsKey(OptimizedImagePath))
                return;

            dictionary.Add(item.Properties[OriginalImagePath].StringValue, item.Properties[OptimizedImagePath].StringValue);
        }
    }
}