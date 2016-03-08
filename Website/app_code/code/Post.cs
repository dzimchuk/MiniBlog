using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using CommonMark;
using Microsoft.Practices.ServiceLocation;
using MiniBlog.Contracts.Framework;
using Util;

public class Post
{
    private static readonly CommonMarkSettings CustomCommonMarkSettings;
    private string htmlContent;
    private string content;

    static Post()
    {
        var optimizedImageService = ServiceLocator.Current.GetInstance<OptimizedImageService>();
        var configuration = ServiceLocator.Current.GetInstance<IConfiguration>();

        CustomCommonMarkSettings = CommonMarkSettings.Default.Clone();
        CustomCommonMarkSettings.OutputDelegate = (doc, output, settings) => new CustomHtmlFormatter(output, settings, optimizedImageService, configuration).WriteDocument(doc);
    }

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
        IsPublished = false;
    }
    
    public string ID { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Slug { get; set; }
    public string Excerpt { get; set; }
    public string Content
    {
        get { return content; }
        set
        {
            content = value;
            htmlContent = null;
        }
    }
    
    public DateTimeOffset PubDate { get; set; }
    public DateTimeOffset LastModified { get; set; }
    public bool IsPublished { get; set; }
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
        return (Blog.ModerateComments && !context.User.Identity.IsAuthenticated) ? Comments.Count(c => c.IsApproved) : this.Comments.Count;
    }

    public string GetHtmlContent()
    {
        return htmlContent ?? (htmlContent = CommonMarkConverter.Convert(Content, CustomCommonMarkSettings));
    }
}