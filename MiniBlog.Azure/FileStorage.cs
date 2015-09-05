using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.WindowsAzure.Storage.Blob;
using MiniBlog.Contracts;

namespace MiniBlog.Azure
{
    internal class FileStorage : IFileStorage
    {
        private readonly IBlobContainerFactory containerFactory;

        private readonly Dictionary<string, string> knownMimeTypes = new Dictionary<string, string>
                                                                     {
                                                                         { "jpeg", "image/jpeg" },
                                                                         { "jpe", "image/jpeg" },
                                                                         { "jpg", "image/jpeg" },
                                                                         { "gif", "image/gif" },
                                                                         { "png", "image/png" },
                                                                         { "bmp", "image/bmp" },
                                                                         { "tiff", "image/tiff" }
                                                                     };

        public FileStorage(IBlobContainerFactory containerFactory)
        {
            this.containerFactory = containerFactory;
        }

        public string Save(byte[] bytes, string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = "bin";
            }

            var blob = Upload(bytes, extension);
            SetProperties(blob, extension);

            return blob.Uri.AbsoluteUri;
        }

        private ICloudBlob Upload(byte[] bytes, string extension)
        {
            var blobName = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", Guid.NewGuid(), extension.Trim('.'));

            var container = containerFactory.Create(Constants.FileContainerKey);
            var blob = container.GetBlockBlobReference(blobName);
            blob.UploadFromByteArray(bytes, 0, bytes.Length);

            return blob;
        }

        private void SetProperties(ICloudBlob blob, string extension)
        {
            blob.FetchAttributes();

            string mimeType;
            if (knownMimeTypes.TryGetValue(extension, out mimeType))
            {
                blob.Properties.ContentType = mimeType;
            }

            blob.Properties.CacheControl = "public, max-age=31536000";
            blob.SetProperties();
        }
    }
}