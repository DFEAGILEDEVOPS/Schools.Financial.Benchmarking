using System;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using SFB.Web.ApplicationCore.Helpers.Constants;

namespace SFB.Web.Infrastructure.Logging
{
    public class NetCoreLogManager : ILogManager
    {
        IHttpContextAccessor _httpContextAccessor;
        private string _enableAiTelemetry;
        public NetCoreLogManager(IHttpContextAccessor httpContextAccessor, string enableAiTelemetry)
        {
            _httpContextAccessor = httpContextAccessor;
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
                    ai.TrackTrace($"URL: {_httpContextAccessor.HttpContext.Request.Path}");
                    ai.TrackTrace($"Data error message: {errorMessage}");
                    //ai.TrackTrace($"FORM VARIABLES: {_httpContextAccessor.HttpContext.Request.Form}");
                    var schoolBmCookie = _httpContextAccessor.HttpContext.Request.Cookies[CookieNames.COMPARISON_LIST];
                    if (schoolBmCookie != null)
                    {
                        ai.TrackTrace($"SCHOOL BM COOKIE: {schoolBmCookie}");
                    }
                    var matBmCookie = _httpContextAccessor.HttpContext.Request.Cookies[CookieNames.COMPARISON_LIST_MAT];
                    if (matBmCookie != null)
                    {
                        ai.TrackTrace($"TRUST BM COOKIE: {matBmCookie}");
                    }
                }
            }
        }
    }
}
