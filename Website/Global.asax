<%@ Application Language="C#" %>
<%@ Import Namespace="LightInject" %>
<%@ Import Namespace="LightInject.ServiceLocation" %>
<%@ Import Namespace="Microsoft.ApplicationInsights" %>
<%@ Import Namespace="Microsoft.ApplicationInsights.Extensibility" %>
<%@ Import Namespace="Microsoft.Practices.ServiceLocation" %>
<%@ Import Namespace="MiniBlog.Contracts.Framework" %>

<script RunAt="server">

    public override string GetVaryByCustomString(HttpContext context, string arg)
    {
        if (arg == "authenticated")
        {
            HttpCookie cookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (cookie != null)
                return cookie.Value;
        }

        return base.GetVaryByCustomString(context, arg);
    }

    public void Application_Start(object sender, EventArgs e)
    {
        ConfigureDependencies();
        ConfigureApplicationInsights();
    }

    private static void ConfigureDependencies()
    {
        var container = new ServiceContainer();
        container.RegisterFrom<MiniBlog.Services.Composition.CompositionModule>();
        container.RegisterFrom<CompositionModule>();
        container.RegisterFrom<MiniBlog.Azure.Composition.CompositionModule>();
        container.RegisterAssembly("MiniBlog.Search.dll");

        ServiceLocator.SetLocatorProvider(() => new LightInjectServiceLocator(container));
    }

    private static void ConfigureApplicationInsights()
    {
        var configuration = ServiceLocator.Current.GetInstance<IConfiguration>();
        TelemetryConfiguration.Active.InstrumentationKey = configuration.Find("appinsights:instrumentationKey");
    }

    public void Application_BeginRequest(object sender, EventArgs e)
    {
        Context.Items["IIS_WasUrlRewritten"] = "false";
        System.Web.WebPages.WebPageHttpHandler.DisableWebPagesResponseHeader = true;

        var application = sender as HttpApplication;
        if (application != null && application.Context != null)
        {
            application.Context.Response.Headers.Remove("Server");
        }
    }

    public void Application_OnError()
    {
        var request = HttpContext.Current.Request;
        var exception = Server.GetLastError();

        if (HttpContext.Current.IsCustomErrorEnabled && exception != null)
        {
            var telemetryClient = new TelemetryClient();
            telemetryClient.TrackException(exception);
        }

        var httpException = exception as HttpException;
        if (httpException == null)
        {
            return;
        }
        
        //Prevents customError behavior when the request is determined to be an AJAX request.
        if (request["X-Requested-With"] == "XMLHttpRequest" || request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            Server.ClearError();
            Response.ClearContent();
            Response.StatusCode = httpException.GetHttpCode();
            Response.StatusDescription = httpException.Message;
            Response.Write(string.Format("<html><body><h1>{0} {1}</h1></body></html>", httpException.GetHttpCode(), httpException.Message));
        }
    }

</script>
