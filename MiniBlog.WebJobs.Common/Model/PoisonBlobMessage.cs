namespace MiniBlog.WebJobs.Common.Model
{
    public class PoisonBlobMessage
    {
        public string FunctionId { get; set; }
        public string BlobType { get; set; }
        public string ContainerName { get; set; }
        public string BlobName { get; set; }
        public string ETag { get; set; }
    }
}