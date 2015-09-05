using Microsoft.WindowsAzure.Storage.Queue;

namespace MiniBlog.Azure
{
    internal interface ICloudQueueFactory
    {
        CloudQueue Create(string queueKey);
    }
}