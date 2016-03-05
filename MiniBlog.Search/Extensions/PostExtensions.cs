using Microsoft.Azure.Search.Models;
using MiniBlog.Contracts.Model;

namespace MiniBlog.Search.Extensions
{
    public static class PostExtensions
    {
        public static Document ToDocument(this Post post)
        {
            return new Document
                   {
                       { "Id", post.Id },
                       { "Title", post.Title },
                       { "Content", post.Content },
                       { "Categories", post.Categories },
                       { "IsPublished", post.IsPublished },
                       { "PubDate", post.PubDate }
                   };
        }
    }
}