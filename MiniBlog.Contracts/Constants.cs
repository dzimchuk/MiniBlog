namespace MiniBlog.Contracts
{
    public static class Constants
    {
        public const string PostDirectory = "posts";
        public const string NotificationFileName = "changes.txt";

        public const string ContentStorageKey = "blog:contentStorage";
        public const string FileContainerKey = "blog:contentContainer";
        public const string PostContainerKey = "blog:postContainer";

        public const string OptimizedImageMap = "blog:optimizedImageMap";

        public const string IndexQueueKey = "search:indexQueue";
        public const string DeleteQueueKey = "search:deleteQueue";
    }
}