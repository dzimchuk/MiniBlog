using System;
using System.Collections.Generic;

namespace MiniBlog.Contracts.Model
{
    public class Post
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Slug { get; set; }
        public string Excerpt { get; set; }
        public string Content { get; set; }
        public DateTime PubDate { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsPublished { get; set; }
        public string[] Categories { get; set; }
        public List<Comment> Comments { get; set; }

        public string GetFileName()
        {
            return $"{Id}{Constants.PostFileExtension}";
        }
    }
}