using Microsoft.WindowsAzure.Storage.Table;

namespace MiniBlog.Azure
{
    public interface ICloudTableFactory
    {
        CloudTable Create(string tableKey);
    }
}