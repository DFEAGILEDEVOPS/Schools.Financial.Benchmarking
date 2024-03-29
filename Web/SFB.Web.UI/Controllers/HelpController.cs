﻿using Microsoft.ApplicationInsights;
using Newtonsoft.Json;
using SFB.Web.ApplicationCore.Helpers.Constants;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace SFB.Web.UI.Controllers
{
    public class HelpController : Controller
    {
        private readonly IEmailSendingService _emailSender;
        public HelpController(IEmailSendingService emailSender)
        {
            _emailSender = emailSender;
        }

        public ActionResult SadGuidance()
        {
            return View();
        }

        public ActionResult DataSources()
        {
            return View();
        }

        public ActionResult DataQueries(long? urn, string schoolName)
        {
            ViewBag.Urn = urn;
            ViewBag.SchoolName = schoolName;
            ViewBag.ModelState = ModelState;
            return View(new DataQueryViewModel());
        }

        [Route("Help/get-involved")]
        public ActionResult GetInvolved()
        {
            ViewBag.ModelState = ModelState;
            return View(new GetInvolvedViewModel());
        }

        public ActionResult Cookies()
        {
            ViewBag.referrer = Request.UrlReferrer;

            var cookiePolicy = new CookiePolicyModel() { Essential = true };
            var cookieId = string.Concat(CookieNames.COOKIE_POLICY, "_");
            var cookie = System.Web.HttpContext.Current.Request.Cookies[cookieId];
            if (cookie != null)
            {
                cookiePolicy = JsonConvert.DeserializeObject<CookiePolicyModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
            }
            return View(cookiePolicy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetInvolvedSubmission(GetInvolvedViewModel getInvolved)
        {
            ViewBag.ModelState = ModelState;
            if (ModelState.IsValid)
            {
                var placeholders = new Dictionary<string, dynamic>{
                    { "Name", FormFieldSanitizer.SanitizeFormField(getInvolved.Name) },
                    { "Email", getInvolved.Email },
                };

                try
                {
                    await _emailSender.SendGetInvolvedEmailAsync(ConfigurationManager.AppSettings["SRMEmailAddress"], placeholders);
                }
                catch (HttpRequestValidationException exc)
                {
                    var enableAITelemetry = WebConfigurationManager.AppSettings["EnableAITelemetry"];

                    if (enableAITelemetry != null && bool.Parse(enableAITelemetry))
                    {
                        var ai = new TelemetryClient();
                        ai.TrackException(exc);
                        ai.TrackTrace($"Get involved email rejected due to SQL injection attack!");
                        ai.TrackTrace($"Contact us email sending failed for: {getInvolved.Name} - ({getInvolved.Email})");
                    }
                    throw exc;
                }
                catch (Exception exception)
                {
                    var enableAITelemetry = WebConfigurationManager.AppSettings["EnableAITelemetry"];

                    if (enableAITelemetry != null && bool.Parse(enableAITelemetry))
                    {
                        var ai = new TelemetryClient();
                        ai.TrackException(exception);
                        ai.TrackTrace($"Get involved email sending error: {exception.Message}");
                        ai.TrackTrace($"Get involved email sending failed for: {getInvolved.Name} ({getInvolved.Email})");
                    }
                    throw;
                }

                SetRecruitmentBannerCookie();
                return View("GetInvolvedConfirmation");
            }
            else
            {
                return View("GetInvolved", getInvolved);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DataQuerySubmission(DataQueryViewModel dataQuery, long? urn, string schoolName)
        {
            ViewBag.ModelState = ModelState;
            if (ModelState.IsValid)
            {
                var emailReference = GenerateEmailReference(dataQuery);
                ViewBag.EmailReference = emailReference;

                var placeholders = new Dictionary<string, dynamic>{
                    { "EmailReference ", emailReference },
                    { "Name", FormFieldSanitizer.SanitizeFormField(dataQuery.Name) },
                    { "Email", dataQuery.Email },
                    { "SchoolTrustName", FormFieldSanitizer.SanitizeFormField(dataQuery.SchoolTrustName) },
                    { "SchoolTrustReferenceNumber", FormFieldSanitizer.SanitizeFormField(dataQuery.SchoolTrustReferenceNumber) },
                    { "DataQuery", dataQuery.DataQuery }
                };

                try
                {
                    await _emailSender.SendDataQueryUserEmailAsync(dataQuery.Email, placeholders);
                    await _emailSender.SendDataQueryDfEEmailAsync(ConfigurationManager.AppSettings["SRMEmailAddress"], placeholders);
                }
                catch (HttpRequestValidationException exc)
                {
                    var enableAITelemetry = WebConfigurationManager.AppSettings["EnableAITelemetry"];

                    if (enableAITelemetry != null && bool.Parse(enableAITelemetry))
                    {
                        var ai = new TelemetryClient();
                        ai.TrackException(exc);
                        ai.TrackTrace($"Data query email rejected due to SQL injection attack!");
                        ai.TrackTrace($"Data query email sending failed for: {dataQuery.Name} - ({dataQuery.Email}) ({dataQuery.SchoolTrustName}) ({dataQuery.SchoolTrustReferenceNumber})");
                    }
                    throw exc;
                }
                catch (Exception exception)
                {
                    var enableAITelemetry = WebConfigurationManager.AppSettings["EnableAITelemetry"];

                    if (enableAITelemetry != null && bool.Parse(enableAITelemetry))
                    {
                        var ai = new TelemetryClient();
                        ai.TrackException(exception);
                        ai.TrackTrace($"Data query email sending error: {exception.Message}");
                        ai.TrackTrace($"Data query email sending failed for: {dataQuery.Name} ({dataQuery.Email}) - ref: {emailReference}");
                    }
                    throw;
                }

                ViewBag.Urn = urn;
                ViewBag.SchoolName = schoolName;
                return View("DataQueryConfirmation");
            }
            else
            {
                ViewBag.Urn = urn;
                ViewBag.SchoolName = schoolName;
                return View("DataQueries", dataQuery);
            }
        }

        private string GenerateEmailReference(DataQueryViewModel dataQuery)
        {
            return $"{dataQuery.SchoolTrustName.Substring(0, 3)}-{DateTime.UtcNow.Minute.ToString()}{DateTime.UtcNow.Second.ToString()}{DateTime.UtcNow.Millisecond.ToString()}";
        }


        private void SetRecruitmentBannerCookie()
        {
            HttpCookie cookie = new HttpCookie(CookieNames.SUPPRES_RECRUITMENT_BANNER, "yes");
            cookie.Expires = DateTime.Now.AddDays(180);
            cookie.HttpOnly = false;
            cookie.Secure = Request.IsSecureConnection;
            Response.Cookies.Add(cookie);
        }
    }
}