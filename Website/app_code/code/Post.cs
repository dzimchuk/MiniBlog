using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using CookComputing.XmlRpc;
using Microsoft.Practices.ServiceLocation;
using MiniBlog.Contracts;
using MiniBlog.Contracts.Framework;

[XmlRpcMissingMapping(MappingAction.Ignore)]
public class Post
{
    private static readonly OptimizedImageService OptimizedImageService = ServiceLocator.Current.GetInstance<OptimizedImageService>();
    private static readonly IConfiguration Configuration = ServiceLocator.Current.GetInstance<IConfiguration>();

    public Post()
    {
        ID = Guid.NewGuid().ToString();
        Title = "My new post";
        Author = ConfigurationManager.AppSettings["blog:author"];
        Content = "the content";
        PubDate = DateTime.UtcNow;
        LastModified = DateTime.UtcNow;
        Categories = new string[0];
        Comments = new List<Comment>();
        IsPublished = true;
    }

    [XmlRpcMember("postid")]
    public string ID { get; set; }

    [XmlRpcMember("title")]
    public string Title { get; set; }

    [XmlRpcMember("author")]
    public string Author { get; set; }

    [XmlRpcMember("wp_slug")]
    public string Slug { get; set; }

    [XmlRpcMember("mt_excerpt")]
    public string Excerpt { get; set; }

    [XmlRpcMember("description")]
    public string Content { get; set; }

    [XmlRpcMember("dateCreated")]
    public DateTime PubDate { get; set; }

    [XmlRpcMember("dateModified")]
    public DateTime LastModified { get; set; }

    public bool IsPublished { get; set; }

    [XmlRpcMember("categories")]
    public string[] Categories { get; set; }
    public List<Comment> Comments { get; private set; }

    public Uri AbsoluteUrl
    {
        get
        {
            Uri requestUrl = HttpContext.Current.Request.Url;
            return new Uri(requestUrl.Scheme + "://" + requestUrl.Authority + Url, UriKind.Absolute);
        }
    }

    public Uri Url
    {
        get
        {
            return new Uri(VirtualPathUtility.ToAbsolute("~/post/" + Slug), UriKind.Relative);
        }
    }

    public bool AreCommentsOpen(HttpContextBase context)
    {
        return PubDate > DateTime.UtcNow.AddDays(-Blog.DaysToComment) || context.User.Identity.IsAuthenticated;
    }

    public int CountApprovedComments(HttpContextBase context)
    {
        return (Blog.ModerateComments && !context.User.Identity.IsAuthenticated) ? this.Comments.Count(c => c.IsApproved) : this.Comments.Count;
    }

    public string GetHtmlContent()
    {
        var result = HandleYoutube(Content);
        result = HandleOptimizedImages(result);

        return result;
    }

    private static string HandleYoutube(string content)
    {
        // Youtube content embedded using this syntax: [youtube:xyzAbc123]
        const string video = "<div class=\"video\"><iframe src=\"//www.youtube.com/embed/{0}?modestbranding=1&amp;theme=light\" allowfullscreen></iframe></div>";
        return Regex.Replace(content, @"\[youtube:(.*?)\]", (Match m) => string.Format(video, m.Groups[1].Value));
    }

    private static string HandleOptimizedImages(string content)
    {
        return Regex.Replace(content, "<img.*?src=\"([^\"]+)\"", (Match m) =>
        {
            var src = m.Groups[1].Value;
            var index = src.IndexOf(Configuration.Find(Constants.FileContainerKey), StringComparison.Ordinal);

            if (index > -1)
            {
                var imagePath = src.Substring(index);
                var optimizedImagePath = OptimizedImageService.FindOptimizedImagePath(imagePath);
                if (!string.IsNullOrEmpty(optimizedImagePath))
                {
                    var leftPart = index > 0 ? src.Substring(0, index) : string.Empty;
                    return m.Value.Replace(src, leftPart + optimizedImagePath);
                }
            }

            return m.Value;
        });
    }
}