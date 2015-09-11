using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace MiniBlog.ImageOptimizer
{
    internal class Compressor
    {
        public async Task<CompressionResult> CompressAsync(ICloudBlob blob)
        {
            string sourceFile = null;
            string targetFile = null;
            try
            {
                sourceFile = Path.GetTempFileName();
                await blob.DownloadToFileAsync(sourceFile, FileMode.Create);

                targetFile = Path.GetTempFileName();
                File.Copy(sourceFile, targetFile, true);

                return new CompressionResult(sourceFile, targetFile);
            }
            catch
            {
                if (!string.IsNullOrEmpty(sourceFile) && File.Exists(sourceFile))
                    File.Delete(sourceFile);

                if (!string.IsNullOrEmpty(targetFile) && File.Exists(targetFile))
                    File.Delete(targetFile);

                throw;
            }
        }
    }
}