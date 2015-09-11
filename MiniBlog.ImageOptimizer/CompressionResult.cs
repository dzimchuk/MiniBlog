using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace MiniBlog.ImageOptimizer
{
    internal class CompressionResult : IDisposable
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

            if (result.Exists)
            {
                ResultFileName = result.FullName;
                ResultFileSize = result.Length;
            }
            else
            {
                throw new Exception($"{resultFileName} does not exist.");
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
            builder.AppendLine("Optimized " + Path.GetFileName(OriginalFileName));
            builder.AppendLine("Before: " + OriginalFileSize + " bytes");
            builder.AppendLine("After: " + ResultFileSize + " bytes");
            builder.AppendLine("Saving: " + Saving + " bytes / " + Percent + "%");

            return builder.ToString();
        }

        public void Dispose()
        {
            if (File.Exists(OriginalFileName))
                File.Delete(OriginalFileName);

            if (File.Exists(ResultFileName))
                File.Delete(ResultFileName);
        }

        public Task UploadResultFileAsync(ICloudBlob blob)
        {
            return blob.UploadFromFileAsync(ResultFileName, FileMode.Open);
        }
    }
}