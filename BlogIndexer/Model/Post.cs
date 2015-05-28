using System;

namespace BlogIndexer.Model
{
    public class Post
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public string[] Categories { get; set; }

        public bool IsPublished { get; set; }
        public DateTimeOffset PubDate { get; set; }
    }
}