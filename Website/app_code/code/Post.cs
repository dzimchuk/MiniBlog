using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using CommonMark;
using CookComputing.XmlRpc;
using Microsoft.Practices.ServiceLocation;
using MiniBlog.Contracts.Framework;
using Util;

[XmlRpcMissingMapping(MappingAction.Ignore)]
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
    public string Content
    {
        get { return content; }
        set
        {
            content = value;
            htmlContent = null;
        }
    }

    [XmlRpcMember("dateCreated")]
    public DateTimeOffset PubDate { get; set; }

    [XmlRpcMember("dateModified")]
    public DateTimeOffset LastModified { get; set; }

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
        if (string.IsNullOrEmpty(htmlContent))
            htmlContent = CommonMarkConverter.Convert(Content, CustomCommonMarkSettings);

        return htmlContent;
    }
}