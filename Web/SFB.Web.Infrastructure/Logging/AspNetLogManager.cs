using Microsoft.ApplicationInsights;
using SFB.Web.ApplicationCore.Helpers.Constants;
using System;
using System.Web;

namespace SFB.Web.Infrastructure.Logging
{
    public class AspNetLogManager : ILogManager
    {
        private string _enableAiTelemetry;
        public AspNetLogManager(string enableAiTelemetry)
        {
            _enableAiTelemetry = enableAiTelemetry;
        }

        public void LogException(Exception exception, string errorMessage)
        {
            if (exception is Newtonsoft.Json.JsonSerializationException || exception is Newtonsoft.Json.JsonReaderException)
            {
                if (_enableAiTelemetry != null && bool.Parse(_enableAiTelemetry))
                {
                    var ai = new TelemetryClient();
                    ai.TrackException(exception);
                    ai.TrackTrace($"URL: {HttpContext.Current?.Request?.RawUrl}");
                    ai.TrackTrace($"Data error message: {errorMessage}");
                    ai.TrackTrace($"FORM VARIABLES: {HttpContext.Current.Request.Form}");
                    var schoolBmCookie = HttpContext.Current.Request.Cookies.Get(CookieNames.COMPARISON_LIST);
                    if (schoolBmCookie != null)
                    {
                        ai.TrackTrace($"SCHOOL BM COOKIE: {schoolBmCookie.Value}");
                    }
                    var matBmCookie = HttpContext.Current.Request.Cookies.Get(CookieNames.COMPARISON_LIST_MAT);
                    if (matBmCookie != null)
                    {
                        ai.TrackTrace($"TRUST BM COOKIE: {matBmCookie.Value}");
                    }
                }
            }
        }
    }
}
