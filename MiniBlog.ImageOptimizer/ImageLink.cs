using Microsoft.WindowsAzure.Storage.Table;

namespace MiniBlog.ImageOptimizer
{
    public class ImageLink : TableEntity
    {
        public string OriginalImagePath { get; set; }
        public string OptimizedImagePath { get; set; }
    }
}