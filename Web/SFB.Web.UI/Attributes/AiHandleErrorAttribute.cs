using System;using System.Web.Mvc;using Microsoft.ApplicationInsights;using SFB.Web.ApplicationCore.Helpers.Constants;
using SFB.Web.ApplicationCore;

namespace SFB.Web.UI.ErrorHandler{    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]     public class AiHandleErrorAttribute : HandleErrorAttribute    {        public override void OnException(ExceptionContext filterContext)        {            if (filterContext?.HttpContext != null && filterContext.Exception != null)            {                //If customError is Off, then AI HTTPModule will report the exception                if (filterContext.HttpContext.IsCustomErrorEnabled)                {
                    if (filterContext.RequestContext.HttpContext.Response.StatusCode != 404)
                    {
                        var ai = new TelemetryClient();
                        ai.TrackException(filterContext.Exception);
                        ai.TrackTrace($"IP ADDRESS: {filterContext.RequestContext.HttpContext.Request.UserHostAddress}");
                        ai.TrackTrace($"HTTP REFERRER: {filterContext.RequestContext.HttpContext.Request.UrlReferrer}");
                        ai.TrackTrace($"BROWSER: {filterContext.RequestContext.HttpContext.Request.Browser.Browser}");
                        ai.TrackTrace($"URL: {filterContext.RequestContext.HttpContext.Request.RawUrl}");
                        ai.TrackTrace($"FORM VARIABLES: {filterContext.RequestContext.HttpContext.Request.Form}");                        var schoolBmCookie = filterContext.RequestContext.HttpContext.Request.Cookies.Get(CookieNames.COMPARISON_LIST);                        if (schoolBmCookie != null)                            ai.TrackTrace(                                $"SCHOOL BM COOKIE: {schoolBmCookie.Value}");                        var matBmCookie = filterContext.RequestContext.HttpContext.Request.Cookies.Get(CookieNames.COMPARISON_LIST_MAT);                        if (matBmCookie !=                            null)                            ai.TrackTrace(                                $"TRUST BM COOKIE: {matBmCookie.Value}");                    }                }             }            //base.OnException(filterContext);        }    }}