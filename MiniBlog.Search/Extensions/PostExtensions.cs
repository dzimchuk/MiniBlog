using System.Text.RegularExpressions;
using Microsoft.Azure.Search.Models;
using MiniBlog.Contracts.Model;

namespace MiniBlog.Search.Extensions
{
    public static class PostExtensions
    {
        private static readonly Regex RegexHtml = new Regex("<.*?>", RegexOptions.Compiled);

        public static Document ToDocument(this Post post)
        {
            return new Document
                   {
                       { "Id", post.Id },
                       { "Title", post.Title },
                       { "Content", RegexHtml.Replace(post.Content, " ") },
                       { "Categories", post.Categories },
                       { "IsPublished", post.IsPublished },
                       { "PubDate", post.PubDate }
                   };
        }
    }
}