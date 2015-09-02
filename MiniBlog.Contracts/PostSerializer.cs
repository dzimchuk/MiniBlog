using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using System.Xml.XPath;
using MiniBlog.Contracts.Model;

namespace MiniBlog.Contracts
{
    public static class PostSerializer
    {
        public static XDocument Serialize(Post post)
        {
            var doc = new XDocument(
                new XElement("post",
                    new XElement("title", post.Title),
                    new XElement("slug", post.Slug),
                    new XElement("author", post.Author),
                    new XElement("pubDate", post.PubDate.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("lastModified", post.LastModified.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("excerpt", post.Excerpt),
                    new XElement("content", post.Content),
                    new XElement("ispublished", post.IsPublished),
                    new XElement("categories", string.Empty),
                    new XElement("comments", string.Empty)
                    ));

            var categories = doc.XPathSelectElement("post/categories");
            foreach (string category in post.Categories)
            {
                categories.Add(new XElement("category", category));
            }

            var comments = doc.XPathSelectElement("post/comments");
            foreach (Comment comment in post.Comments)
            {
                comments.Add(
                    new XElement("comment",
                        new XElement("author", comment.Author),
                        new XElement("email", comment.Email),
                        new XElement("website", comment.Website),
                        new XElement("ip", comment.Ip),
                        new XElement("userAgent", comment.UserAgent),
                        new XElement("date", comment.PubDate.ToString("yyyy-MM-dd HH:m:ss")),
                        new XElement("content", comment.Content),
                        new XAttribute("isAdmin", comment.IsAdmin),
                        new XAttribute("isApproved", comment.IsApproved),
                        new XAttribute("id", comment.Id)
                        ));
            }

            return doc;
        }

        public static Post Deserialize(XElement doc, string postId)
        {
            var post = new Post()
                       {
                           Id = postId,
                           Title = ReadValue(doc, "title"),
                           Author = ReadValue(doc, "author"),
                           Excerpt = ReadValue(doc, "excerpt"),
                           Content = ReadValue(doc, "content"),
                           Slug = ReadValue(doc, "slug").ToLowerInvariant(),
                           PubDate = DateTime.Parse(ReadValue(doc, "pubDate")),
                           LastModified = DateTime.Parse(ReadValue(doc, "lastModified", DateTime.Now.ToString(CultureInfo.InvariantCulture))),
                           IsPublished = bool.Parse(ReadValue(doc, "ispublished", "true")),
                           Categories = new string[0],
                           Comments = new List<Comment>()
                       };

            LoadCategories(post, doc);
            LoadComments(post, doc);

            return post;
        }

        private static void LoadCategories(Post post, XElement doc)
        {
            var categories = doc.Element("categories");
            if (categories == null)
                return;

            var list = new List<string>();

            foreach (var node in categories.Elements("category"))
            {
                list.Add(node.Value);
            }

            post.Categories = list.ToArray();
        }

        private static void LoadComments(Post post, XElement doc)
        {
            var comments = doc.Element("comments");

            if (comments == null)
                return;

            foreach (var node in comments.Elements("comment"))
            {
                var comment = new Comment()
                              {
                                  Id = ReadAttribute(node, "id"),
                                  Author = ReadValue(node, "author"),
                                  Email = ReadValue(node, "email"),
                                  Website = ReadValue(node, "website"),
                                  Ip = ReadValue(node, "ip"),
                                  UserAgent = ReadValue(node, "userAgent"),
                                  IsAdmin = bool.Parse(ReadAttribute(node, "isAdmin", "false")),
                                  IsApproved = bool.Parse(ReadAttribute(node, "isApproved", "true")),
                                  Content = ReadValue(node, "content").Replace("\n", "<br />"),
                                  PubDate = DateTime.Parse(ReadValue(node, "date", "2000-01-01")),
                              };

                post.Comments.Add(comment);
            }
        }

        private static string ReadValue(XElement doc, XName name, string defaultValue = "")
        {
            if (doc.Element(name) != null)
                return doc.Element(name).Value;

            return defaultValue;
        }

        private static string ReadAttribute(XElement element, XName name, string defaultValue = "")
        {
            if (element.Attribute(name) != null)
                return element.Attribute(name).Value;

            return defaultValue;
        }
    }
}