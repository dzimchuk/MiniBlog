using Microsoft.WindowsAzure.Storage.Blob;

namespace MiniBlog.Azure
{
    public interface IBlobContainerFactory
    {
        CloudBlobContainer Create(string containerKey);
    }
}