using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace MiniBlog.ImageOptimizer
{
    internal class CompressionResult
    {
        public CompressionResult(string originalFileName, string resultFileName)
        {
            var original = new FileInfo(originalFileName);
            var result = new FileInfo(resultFileName);

            if (original.Exists)
            {
                OriginalFileName = original.FullName;
                OriginalFileSize = original.Length;
            }
            else
            {
                throw new Exception($"{originalFileName} does not exist.");
            }

            if (result.Exists && result.Length > 0L)
            {
                ResultFileName = result.FullName;
                ResultFileSize = result.Length;
            }
            else
            {
                throw new Exception($"{resultFileName} does not exist or is empty.");
            }
        }

        private long OriginalFileSize { get; }
        private string OriginalFileName { get; }
        private long ResultFileSize { get; }
        private string ResultFileName { get; }

        public long Saving => OriginalFileSize - ResultFileSize;

        public double Percent => Math.Round(100 - (double)ResultFileSize / (double)OriginalFileSize * 100, 1);

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Before: " + OriginalFileSize + " bytes");
            builder.AppendLine("After: " + ResultFileSize + " bytes");
            builder.AppendLine("Saving: " + Saving + " bytes / " + Percent + "%");

            return builder.ToString();
        }

        public Task UploadResultFileAsync(ICloudBlob blob)
        {
            return blob.UploadFromFileAsync(ResultFileName, FileMode.Open);
        }
    }
}