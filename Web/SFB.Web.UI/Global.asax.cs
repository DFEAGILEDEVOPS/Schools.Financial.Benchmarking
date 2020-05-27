using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.ApplicationInsights.Extensibility;
using System.Linq;
using System.Net;

namespace SFB.Web.UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            IocConfig.Register();

            MvcHandler.DisableMvcResponseHeader = true;

            var enableAITelemetry = ConfigurationManager.AppSettings["EnableAITelemetry"];
            TelemetryConfiguration.Active.DisableTelemetry = enableAITelemetry == null || !bool.Parse(enableAITelemetry);

            if (ServicePointManager.SecurityProtocol.HasFlag(SecurityProtocolType.Tls12) == false) 
            {
                ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var staticExtensions = new[] { ".png", ".ico", ".gif", ".css", ".map", ".js" };
            if (staticExtensions.Contains(Request.CurrentExecutionFilePathExtension))
            {
                Response.Cache.AppendCacheExtension("pre-check=31536000");
                Response.AppendHeader("Cache-Control", "max-age=31536000");
            }
            else
            {
                //Response.Cache.SetCacheability(HttpCacheability.Server); //Caches the pages unnecessarily when auth enabled. Therefore commented out.
                Response.Cache.AppendCacheExtension("no-cache, no-store, must-revalidate");
                Response.AppendHeader("Pragma", "no-cache"); // HTTP 1.0.
                Response.AppendHeader("Expires", "0"); // Proxies.
            }

            Response.AddHeader("x-frame-options", "SAMEORIGIN");
            Response.AppendHeader("Strict-Transport-Security", "max-age=31536000"); // HSTS.
            Response.AppendHeader("X-XSS-Protection", "1; mode=block");
            Response.AppendHeader("X-Content-Type-Options", "nosniff");
        }

        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");
        }

    }
}
