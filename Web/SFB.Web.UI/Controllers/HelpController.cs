using Microsoft.ApplicationInsights;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.UI.Models;
using System;
using System.Collections.Generic;
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

        public ActionResult Guidance()
        {
            return View();
        }

        public ActionResult InterpretingCharts()
        {
            return View();
        }

        public ActionResult DataSources()
        {
            return View();
        }

        public ActionResult DataQueries()
        {
            ViewBag.ModelState = ModelState;
            return View(new DataQueryViewModel());
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> DataQuerySubmission(DataQueryViewModel dataQuery)
        {
            ViewBag.ModelState = ModelState;
            if (ModelState.IsValid)
            {
                var emailReference = GenerateEmailReference(dataQuery);
                ViewBag.EmailReference = emailReference;

                var placeholders = new Dictionary<String, dynamic>{
                    { "EmailReference ", emailReference },
                    { "Name", dataQuery.Name },
                    { "Email", dataQuery.Email },
                    { "SchoolTrustName", dataQuery.SchoolTrustName },
                    { "SchoolTrustReferenceNumber", dataQuery.SchoolTrustReferenceNumber },
                    { "DataQuery", dataQuery.DataQuery }
                };

                try
                {
                    var userEmailResponse = await _emailSender.SendUserEmailAsync(dataQuery.Email, placeholders);
                    var dfeEmailResponse = await _emailSender.SendDfEEmailAsync("school.resourcemanagement@education.gov.uk", placeholders);
                }
                catch (Exception exception)
                {
                    var enableAITelemetry = WebConfigurationManager.AppSettings["EnableAITelemetry"];

                    if (enableAITelemetry != null && bool.Parse(enableAITelemetry))
                    {
                        var ai = new TelemetryClient();
                        ai.TrackException(exception);
                        ai.TrackTrace($"Email sending error: {exception.Message}");
                        ai.TrackTrace($"Email sending failed for: {dataQuery.Name} ({dataQuery.Email}) - ref: {emailReference}");
                    }
                    throw;
                }

                return View("DataQueryConfirmation");
            }
            else
            {                
                return View("DataQueries", dataQuery);
            }
        }

        private string GenerateEmailReference(DataQueryViewModel dataQuery)
        {
            return $"{dataQuery.SchoolTrustName.Substring(0, 3)}-{DateTime.UtcNow.Minute.ToString()}{DateTime.UtcNow.Second.ToString()}{DateTime.UtcNow.Millisecond.ToString()}";
        }
    }
}