using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace MiniBlog.ImageOptimizer
{
    internal class Compressor : IDisposable
    {
        private static readonly string[] SupportedExtensions = { ".png", ".jpg", ".jpeg", ".gif" };

        private readonly ICloudBlob blob;
        private string sourceFile;
        private string targetFile;

        public Compressor(ICloudBlob blob)
        {
            this.blob = blob;
        }

        public bool IsSupported
        {
            get
            {
                var sourceExtension = SourceExtension;
                return SupportedExtensions.Any(ext => ext.Equals(sourceExtension, StringComparison.OrdinalIgnoreCase));
            }
        }

        public async Task<CompressionResult> CompressAsync()
        {
            if (!IsSupported)
                throw new NotSupportedException($"{blob.Name} is not a supported file type.");

            sourceFile = Path.GetTempFileName();
            await blob.DownloadToFileAsync(sourceFile, FileMode.Create);

            targetFile = Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Path.GetRandomFileName(), SourceExtension));
            Compress();

            return new CompressionResult(sourceFile, targetFile);
        }

        private string SourceExtension => Path.GetExtension(blob.Name);

        private void Compress()
        {
            var start = new ProcessStartInfo("cmd")
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), @"Resources\"),
                Arguments = GetArguments(),
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var process = new Process { StartInfo = start };
            process.Start();
            process.WaitForExit();
        }

        private string GetArguments()
        {
            var extension = SourceExtension.ToLowerInvariant();
            switch (extension)
            {
                case ".png":
                    return string.Format(CultureInfo.InvariantCulture, "/c pngout \"{0}\" \"{1}\" /s0 /y /v /kpHYs /force", sourceFile, targetFile);

                case ".jpg":
                case ".jpeg":
                    return string.Format(CultureInfo.InvariantCulture, "/c jpegtran -copy none -optimize -progressive \"{0}\" \"{1}\"", sourceFile, targetFile);

                case ".gif":
                    return string.Format(CultureInfo.InvariantCulture, "/c gifsicle --no-comments --no-extensions --no-names --optimize=3 --batch \"{0}\" --output=\"{1}\"", sourceFile, targetFile);
                default:
                    return null;
            }
        }

        public void Dispose()
        {
            if (!string.IsNullOrEmpty(sourceFile) && File.Exists(sourceFile))
                File.Delete(sourceFile);

            if (!string.IsNullOrEmpty(targetFile) && File.Exists(targetFile))
                File.Delete(targetFile);
        }
    }
}