using System;
using Microsoft.Azure.Search;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace MiniBlog.Search.Extensions
{
    internal class SearchIndexErrorDetectionStrategy : ITransientErrorDetectionStrategy
    {
        public bool IsTransient(Exception ex)
        {
            return ex is IndexBatchException;
        }
    }
}