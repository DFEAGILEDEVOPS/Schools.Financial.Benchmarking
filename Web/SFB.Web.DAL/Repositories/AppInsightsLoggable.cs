using Microsoft.ApplicationInsights;
using SFB.Web.ApplicationCore.Helpers.Constants;
using SFB.Web.ApplicationCore;
using System;
using System.Diagnostics;
using System.Web;
using System.Web.Configuration;

namespace SFB.Web.Infrastructure.Repositories
{
    public abstract class AppInsightsLoggable
    {
        internal virtual void LogException(Exception exception, string errorMessage)
        {
            Debugger.Break();

            if (exception is Newtonsoft.Json.JsonSerializationException || exception is Newtonsoft.Json.JsonReaderException)
            {
                var enableAITelemetry = WebConfigurationManager.AppSettings["EnableAITelemetry"];

                if (enableAITelemetry != null && bool.Parse(enableAITelemetry))
                {
                    var ai = new TelemetryClient();
                    ai.TrackException(exception);
                    ai.TrackTrace($"URL: {HttpContext.Current.Request.RawUrl}");
                    ai.TrackTrace($"Data error message: {errorMessage}");
                    ai.TrackTrace($"FORM VARIABLES: {HttpContext.Current.Request.Form}");
                    var schoolBmCookie = HttpContext.Current.Request.Cookies.Get(CookieNames.COMPARISON_LIST);
                    if (schoolBmCookie != null)
                        ai.TrackTrace(
                            $"SCHOOL BM COOKIE: {schoolBmCookie.Value}");
                    var matBmCookie = HttpContext.Current.Request.Cookies.Get(CookieNames.COMPARISON_LIST_MAT);
                    if (matBmCookie != null)
                        ai.TrackTrace(
                            $"TRUST BM COOKIE: {matBmCookie.Value}");

                }
            }
        }
    }
}
