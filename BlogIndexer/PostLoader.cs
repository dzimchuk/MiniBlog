using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using BlogIndexer.Model;

namespace BlogIndexer
{
    internal class PostLoader
    {
        private readonly string directory;

        public PostLoader(string directory)
        {
            this.directory = directory;
        }

        public List<Post> LoadPosts()
        {
            var result = new List<Post>();

            foreach (var file in Directory.EnumerateFiles(directory, "*.xml", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    var post = LoadPost(file);
                    result.Add(post);
                }
                catch
                {
                    Console.WriteLine("Error loading post {0}", Path.GetFileNameWithoutExtension(file));
                    throw;
                }
            }

            return result;
        }

        private static readonly Regex RegexHtml = new Regex("<.*?>", RegexOptions.Compiled);

        private static Post LoadPost(string file)
        {
            var doc = XElement.Load(file);

            var post = new Post
                       {
                           Id = Path.GetFileNameWithoutExtension(file),
                           Title = ReadValue(doc, "title"),
                           Content = RegexHtml.Replace(ReadValue(doc, "content"), " "),
                           PubDate = ParseDate(ReadValue(doc, "pubDate")),
                           IsPublished = bool.Parse(ReadValue(doc, "ispublished", "true"))
                       };

            LoadCategories(post, doc);
            return post;
        }

        private static readonly Regex RegexDate = 
            new Regex("^(?<year>\\d{4})-(?<month>\\d{2})-(?<day>\\d{2})\\s+(?<hour>\\d{2}):(?<minute>\\d{2}):(?<second>\\d{2})$", RegexOptions.Compiled);

        private static DateTimeOffset ParseDate(string value)
        {
            var m = RegexDate.Match(value);
            if (!m.Success)
                throw new Exception("Failed to parse date");

            return new DateTimeOffset(int.Parse(m.Groups["year"].Value),
                int.Parse(m.Groups["month"].Value),
                int.Parse(m.Groups["day"].Value),
                int.Parse(m.Groups["hour"].Value),
                int.Parse(m.Groups["minute"].Value),
                int.Parse(m.Groups["second"].Value),
                TimeSpan.Zero);
        }

        private static void LoadCategories(Post post, XElement doc)
        {
            var categories = doc.Element("categories");
            if (categories == null)
                return;

            post.Categories = categories.Elements("category").Select(node => node.Value).ToArray();
        }

        private static string ReadValue(XElement doc, XName name, string defaultValue = "")
        {
            if (doc.Element(name) != null)
                return doc.Element(name).Value;

            return defaultValue;
        }
    }
}