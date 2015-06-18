using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json;

public class SuggestHandler : IHttpHandler
{
    private const string ContentTypeJson = "application/json";

    public void ProcessRequest(HttpContext context)
    {
        // TODO: anti forgery validation (see PostHandler)

        var searchText = context.Request.QueryString["term"];
        if (string.IsNullOrWhiteSpace(searchText) || searchText.Length < 3)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NoContent;
            return;
        }

        var result = SearchFacade.Suggest(searchText);
        if (result != null)
        {
            var items = result.Select(i => i.Text).Distinct();

            context.Response.ContentType = ContentTypeJson;
            context.Response.Output.Write(JsonConvert.SerializeObject(items));
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.NoContent;
        }
    }

    public bool IsReusable { get { return false; } }
}