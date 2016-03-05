using System;
using System.IO;
using CommonMark;
using CommonMark.Formatters;
using CommonMark.Syntax;
using MiniBlog.Contracts;
using MiniBlog.Contracts.Framework;

namespace Util
{
    internal class CustomHtmlFormatter : HtmlFormatter
    {
        private readonly OptimizedImageService optimizedImageService;
        private readonly IConfiguration configuration;

        public CustomHtmlFormatter(TextWriter target, CommonMarkSettings settings, OptimizedImageService optimizedImageService, IConfiguration configuration) : base(target, settings)
        {
            this.optimizedImageService = optimizedImageService;
            this.configuration = configuration;
        }

        protected override void WriteInline(Inline inline, bool isOpening, bool isClosing, out bool ignoreChildNodes)
        {
            if (inline.Tag == InlineTag.Link && !RenderPlainTextInlines.Peek())
            {
                ignoreChildNodes = false;

                if (isOpening)
                {
                    Write("<a target=\"_blank\" href=\"");
                    WriteEncodedUrl(inline.TargetUrl);
                    Write("\">");
                }

                if (isClosing)
                {
                    Write("</a>");
                }
            }
            else if (inline.Tag == InlineTag.Image && !RenderPlainTextInlines.Peek())
            {
                ignoreChildNodes = true;

                if (isOpening)
                {
                    Write("<img class=\"img-responsive center-block\" src=\"");
                    WriteEncodedUrl(GetImageLink(inline.TargetUrl));
                    Write("\"");

                    if (inline.FirstChild != null && !string.IsNullOrWhiteSpace(inline.FirstChild.LiteralContent))
                    {
                        Write(" alt=\"");
                        Write(inline.FirstChild.LiteralContent);
                        Write("\"");
                    }

                    Write(" />");
                }
            }
            else
            {
                base.WriteInline(inline, isOpening, isClosing, out ignoreChildNodes);
            }
        }

        private string GetImageLink(string originalLink)
        {
            var index = originalLink.IndexOf(configuration.Find(Constants.FileContainerKey), StringComparison.Ordinal);
            if (index > -1)
            {
                var imagePath = originalLink.Substring(index);
                var optimizedImagePath = optimizedImageService.FindOptimizedImagePath(imagePath);
                if (!string.IsNullOrEmpty(optimizedImagePath))
                {
                    var leftPart = index > 0 ? originalLink.Substring(0, index) : string.Empty;
                    return leftPart + optimizedImagePath;
                }
            }

            return originalLink;
        }
    }
}