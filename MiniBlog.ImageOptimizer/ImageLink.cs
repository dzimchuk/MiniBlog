using Microsoft.WindowsAzure.Storage.Table;

namespace MiniBlog.ImageOptimizer
{
    public class ImageLink : TableEntity
    {
        public string OptimizedImagePath { get; set; }
    }
}