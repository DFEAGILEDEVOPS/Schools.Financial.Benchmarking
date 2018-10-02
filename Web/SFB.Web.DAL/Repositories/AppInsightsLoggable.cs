using Microsoft.ApplicationInsights;
using Microsoft.Azure.Documents;
using SFB.Web.Common;
using System;
using System.Diagnostics;
using System.Web;
using System.Web.Configuration;

namespace SFB.Web.DAL.Repositories
{
    public abstract class AppInsightsLoggable
    {
        internal void LogException(Exception exception, string errorMessage)
        {
            if (exception is Newtonsoft.Json.JsonSerializationException || exception is Newtonsoft.Json.JsonReaderException)
            {                
                if (bool.Parse(WebConfigurationManager.AppSettings["EnableElmahLogs"]))
                {
                    if (HttpContext.Current.IsCustomErrorEnabled)
                    {
                        var ai = new TelemetryClient();
                        ai.TrackException(exception);
                        ai.TrackTrace($"URL: {HttpContext.Current.Request.RawUrl}");
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
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new ApplicationException(errorMessage));
                }
                else
                {
                    Debug.WriteLine(errorMessage);
                }
            }
        }
    }
}
