﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.Helpers;
using System.Web.Hosting;
using Microsoft.Practices.ServiceLocation;
using MiniBlog.Contracts;

public static class Blog
{
    private static readonly IStorageAdapter Storage = ServiceLocator.Current.GetInstance<IStorageAdapter>();

    static Blog()
    {
        Title = ConfigurationManager.AppSettings.Get("blog:name");
        Description = ConfigurationManager.AppSettings.Get("blog:description");
        PostsPerPage = int.Parse(ConfigurationManager.AppSettings.Get("blog:postsPerPage"));
        DaysToComment = int.Parse(ConfigurationManager.AppSettings.Get("blog:daysToComment"));
        Image = ConfigurationManager.AppSettings.Get("blog:image");
        ModerateComments = bool.Parse(ConfigurationManager.AppSettings.Get("blog:moderateComments"));
        BlogPath = ConfigurationManager.AppSettings.Get("blog:path");
    }

    public static string Theme 
    {
        get
        {
            return HttpContext.Current.User.Identity.IsAuthenticated
                ? ConfigurationManager.AppSettings.Get("blog:authenticated:theme")
                : ConfigurationManager.AppSettings.Get("blog:theme");
        }
    }

    public static string Title { get; private set; }
    public static string Description { get; private set; }
    public static string Image { get; private set; }
    public static int PostsPerPage { get; private set; }
    public static int DaysToComment { get; private set; }
    public static bool ModerateComments { get; private set; }
    public static string BlogPath { get; private set; }

    public static string CurrentSlug
    {
        get { return (HttpContext.Current.Request.QueryString["slug"] ?? string.Empty).Trim().ToLowerInvariant(); }
    }

    public static string CurrentCategory
    {
        get { return WebUtility.UrlDecode(HttpContext.Current.Request.QueryString["category"] ?? string.Empty).Trim().ToLowerInvariant(); }
    }

    public static bool IsNewPost
    {
        get
        {
            return HttpContext.Current.Request.RawUrl.Trim('/') == (!string.IsNullOrWhiteSpace(BlogPath) ? BlogPath + "/" : "") + "post/new";
        }
    }

    public static bool IsEditing
    {
        get
        {
            return HttpContext.Current.Request.QueryString["mode"] == "edit";
        }
    }

    public static Post CurrentPost
    {
        get
        {
            if (HttpContext.Current.Items["currentpost"] == null)
            {
                var post = FindCurrentPost();
                if (post != null)
                    HttpContext.Current.Items["currentpost"] = post;
            }

            return HttpContext.Current.Items["currentpost"] as Post;
        }
    }

    private static Post FindCurrentPost()
    {
        if (string.IsNullOrEmpty(CurrentSlug))
            return null;

        var post = GetVisiblePosts().FirstOrDefault(p => p.Slug == CurrentSlug);
        if (post == null)
        {
            var previewId = HttpContext.Current.Request.QueryString["key"];
            if (!string.IsNullOrEmpty(previewId))
            {
                post = Storage.GetAllPosts().FirstOrDefault(p => p.Slug == CurrentSlug && p.ID.Equals(previewId, StringComparison.OrdinalIgnoreCase));
            }
        }

        return post;
    }

    public static string GetNextPage()
    {
        if (!string.IsNullOrEmpty(CurrentSlug))
        {
            var posts = GetVisiblePosts().ToList();
            var current = posts.IndexOf(CurrentPost);
            if (current > 0)
                return posts[current - 1].Url.ToString();
        }
        else if (CurrentPage > 1)
        {
            return GetPagingUrl(-1);
        }

        return null;
    }

    public static string GetPrevPage()
    {
        if (!string.IsNullOrEmpty(CurrentSlug))
        {
            var posts = GetVisiblePosts().ToList();
            var current = posts.IndexOf(CurrentPost);
            if (current > -1 && posts.Count > current + 1)
                return posts[current + 1].Url.ToString();
        }
        else if(GetPosts().Count() > PostsPerPage * CurrentPage)
        {
            return GetPagingUrl(1);
        }

        return null;
    }

    public static int CurrentPage
    {
        get
        {
            int page = 0;
            if (int.TryParse(HttpContext.Current.Request.QueryString["page"], out page))
                return page;

            return 1;
        }
    }

    public static IEnumerable<Post> GetPosts(int postsPerPage = 0)
    {
        var posts = GetVisiblePosts();

        var category = CurrentCategory;

        if (!string.IsNullOrEmpty(category))
        {
            posts = posts.Where(p => p.Categories.Any(c => string.Equals(c, category, StringComparison.OrdinalIgnoreCase)));
        }

        if (postsPerPage > 0)
        {
            posts = posts.Skip(postsPerPage * (CurrentPage - 1)).Take(postsPerPage);
        }

        return posts;
    }

    public static void ValidateToken(HttpContext context)
    {
        AntiForgery.Validate();
    }

    public static string SaveFileToDisk(byte[] bytes, string extension)
    {
        var fileStorage = ServiceLocator.Current.GetInstance<IFileStorage>();
        return fileStorage.Save(bytes, extension);
    }

    public static string GetPagingUrl(int move)
    {
        string url = "/page/{0}/";
        string category = CurrentCategory;

        if (!string.IsNullOrEmpty(category))
        {
            url = "/category/" + HttpUtility.UrlEncode(category.ToLowerInvariant()) + "/" + url;
        }

        string relative = string.Format("~" + url, Blog.CurrentPage + move);
        return VirtualPathUtility.ToAbsolute(relative);
    }

    public static string FingerPrint(string rootRelativePath, string cdnPath = "")
    {
        if ( HttpContext.Current.Request.IsLocal && String.IsNullOrWhiteSpace( Blog.BlogPath ) )
            return rootRelativePath;

        if (!string.IsNullOrEmpty(cdnPath) && !HttpContext.Current.IsDebuggingEnabled)
            return cdnPath;

        if (HttpRuntime.Cache[rootRelativePath] == null)
        {
            string relative = VirtualPathUtility.ToAbsolute("~" + rootRelativePath);
            string absolute = HostingEnvironment.MapPath(relative);

            if (!File.Exists(absolute))
                throw new FileNotFoundException("File not found", absolute);

            DateTime date = File.GetLastWriteTime(absolute);
            int index = relative.LastIndexOf('.');

            string result = ConfigurationManager.AppSettings.Get("blog:cdnUrl") + relative.Insert(index, "_" + date.Ticks);

            HttpRuntime.Cache.Insert(rootRelativePath, result, new CacheDependency(absolute));
        }

        return HttpRuntime.Cache[rootRelativePath] as string;
    }

    public static void SetConditionalGetHeaders(DateTimeOffset lastModified, HttpContextBase context)
    {
        HttpResponseBase response = context.Response;
        HttpRequestBase request = context.Request;
        lastModified = new DateTimeOffset(lastModified.Year, lastModified.Month, lastModified.Day, lastModified.Hour, lastModified.Minute, lastModified.Second, lastModified.Offset);

        string incomingDate = request.Headers["If-Modified-Since"];

        response.Cache.SetLastModified(lastModified.DateTime);

        DateTime testDate = DateTime.MinValue;

        if (DateTime.TryParse(incomingDate, out testDate) && testDate == lastModified)
        {
            response.ClearContent();
            response.StatusCode = (int)System.Net.HttpStatusCode.NotModified;
            response.SuppressContent = true;
        }
    }

    public static Dictionary<string, int> GetCategories()
    {
        var result = new Dictionary<string, int>();
        
        foreach (var category in GetVisiblePosts().SelectMany(post => post.Categories))
        {
            if (!result.ContainsKey(category))
                result.Add(category, 0);

            result[category] = result[category] + 1;
        }
        
        return result;
    }

    public static IEnumerable<Post> GetRecentPosts(int count)
    {
        return GetVisiblePosts().Take(count).ToList();
    }

    private static IEnumerable<Post> GetVisiblePosts()
    {
        return Storage.GetAllPosts()
                      .Where(p => ((p.IsPublished && p.PubDate <= DateTime.UtcNow) || HttpContext.Current.User.Identity.IsAuthenticated));
    }

    public static void ClearStartPageCache()
    {
        HttpResponse.RemoveOutputCacheItem(string.Format("/{0}", BlogPath));
    }
}
