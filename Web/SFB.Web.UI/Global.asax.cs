using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.ApplicationInsights.Extensibility;
using System.Linq;
using System.Net;
using Sentry;
using Sentry.AspNet;
using Sentry.EntityFramework;

namespace SFB.Web.UI
{
    public class MvcApplication : System.Web.HttpApplication
    {

        private IDisposable _sentry;

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

            // Initialize Sentry to capture AppDomain unhandled exceptions and more.
            _sentry = SentrySdk.Init(o =>
            {
                o.AddAspNet();
                o.Dsn = "https://c5926ef609174b0e6e274dd675cf3405@o4505662861017088.ingest.sentry.io/4505662864293888";
                // When configuring for the first time, to see what the SDK is doing:
                o.Debug = true;
                // Set TracesSampleRate to 1.0 to capture 100%
                // of transactions for performance monitoring.
                // We recommend adjusting this value in production
                o.TracesSampleRate = 1.0;
                // If you are using EF (and installed the NuGet package):
                o.AddEntityFramework();
            });

        }

        // Global error catcher
        protected void Application_Error() => Server.CaptureLastError();

        protected void Application_BeginRequest()
        {
            Context.StartSentryTransaction();
        }

        protected void Application_EndRequest()
        {
            Context.FinishSentryTransaction();
        }

        protected void Application_End()
        {
            // Flushes out events before shutting down.
            _sentry?.Dispose();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var staticExtensions = new[] { ".svg", ".png", ".ico", ".gif", ".css", ".map", ".js" };
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
