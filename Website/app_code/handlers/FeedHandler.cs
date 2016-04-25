using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;

public class FeedHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        SyndicationFeed feed = new SyndicationFeed()
        {
            Title = new TextSyndicationContent(Blog.Title),
            Description = new TextSyndicationContent("Latest blog posts"),
            BaseUri = new Uri(context.Request.Url.Scheme + "://" + context.Request.Url.Authority),
            Items = GetItems(),
        };

        feed.Links.Add(new SyndicationLink(feed.BaseUri));

        using (var writer = new XmlTextWriter(context.Response.Output))
        {
            var formatter = GetFormatter(context, feed);
            writer.Formatting = Formatting.Indented;
            formatter.WriteTo(writer);
        }

        context.Response.ContentType = "text/xml";
    }

    private IEnumerable<SyndicationItem> GetItems()
    {
        foreach (Post p in Blog.GetPosts(10))
        {
            var item = new SyndicationItem
                       {
                           Id = p.AbsoluteUrl.ToString(),
                           Title = new TextSyndicationContent(p.Title),
                           Content = new TextSyndicationContent(p.GetHtmlContent(), TextSyndicationContentKind.Html),
                           LastUpdatedTime = p.LastModified,
                           PublishDate = p.PubDate,
                           Summary = new TextSyndicationContent(p.GetHtmlContent(), TextSyndicationContentKind.Html)
                       };

            item.Links.Add(SyndicationLink.CreateAlternateLink(p.AbsoluteUrl));
            item.Authors.Add(new SyndicationPerson("", p.Author, ""));
            yield return item;
        }
    }

    private SyndicationFeedFormatter GetFormatter(HttpContext context, SyndicationFeed feed)
    {
        string path = context.Request.Path.Trim('/');
        int index = path.LastIndexOf('/');

        if (index > -1 && path.Substring(index + 1) == "atom")
        {
            context.Response.ContentType = "application/atom+xml";
            return new Atom10FeedFormatter(feed);
        }

        context.Response.ContentType = "application/rss+xml";
        return new Rss20FeedFormatter(feed);
    }

    public bool IsReusable
    {
        get { return false; }
    }
}