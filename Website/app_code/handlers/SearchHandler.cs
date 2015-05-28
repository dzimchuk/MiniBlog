using System.IO;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SearchHandler : IHttpHandler
{
    private const string ContentTypeJson = "application/json";

    public void ProcessRequest(HttpContext context)
    {
        // TODO: anti forgery validation (see PostHandler)

        var request = ParseRequest(context);
        if (request == null)
            return;

        var result = SearchFacade.Search((string)request.SelectToken("search"));
        if (result != null)
        {
            context.Response.ContentType = ContentTypeJson;
            context.Response.Output.Write(JsonConvert.SerializeObject(result));
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.NoContent;
        }
    }

    private static JObject ParseRequest(HttpContext context)
    {
        if (!context.Request.ContentType.Equals(ContentTypeJson, System.StringComparison.OrdinalIgnoreCase)) 
            return null;
        
        using (var reader = new StreamReader(context.Request.InputStream))
        {
            return JObject.Parse(reader.ReadToEnd());
        }
    }

    public bool IsReusable { get { return false; } }
}