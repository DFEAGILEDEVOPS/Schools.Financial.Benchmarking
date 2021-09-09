using Microsoft.ApplicationInsights;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace SFB.Web.UI.Controllers
{
    public class ContactController : Controller
    {
        private readonly IEmailSendingService _emailSender;

        public ContactController(IEmailSendingService emailSender)
        {
            _emailSender = emailSender;
        }

        // GET: Contact
        public ActionResult Index()
        {
            ViewBag.ModelState = ModelState;
            return View(new ContactUsViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ContactUsSubmission(ContactUsViewModel contactUs)
        {
            ViewBag.ModelState = ModelState;
            if (ModelState.IsValid)
            {
                var emailReference = GenerateEmailReference(contactUs);
                ViewBag.EmailReference = emailReference;

                var placeholders = new Dictionary<string, dynamic>{
                    { "EmailReference ", emailReference },
                    { "Name", contactUs.Name },
                    { "Email", contactUs.Email },
                    { "SchoolTrustName", contactUs.SchoolTrustName },
                    { "Message", contactUs.Message }
                };

                try
                {
                    await _emailSender.SendContactUsUserEmailAsync(contactUs.Email, placeholders);
                    await _emailSender.SendContactUsDfEEmailAsync(ConfigurationManager.AppSettings["SRMEmailAddress"], placeholders);
                }
                catch (Exception exception)
                {
                    var enableAITelemetry = WebConfigurationManager.AppSettings["EnableAITelemetry"];

                    if (enableAITelemetry != null && bool.Parse(enableAITelemetry))
                    {
                        var ai = new TelemetryClient();
                        ai.TrackException(exception);
                        ai.TrackTrace($"Contact us email sending error: {exception.Message}");
                        ai.TrackTrace($"Contact us email sending failed for: {contactUs.Name} ({contactUs.Email}) - ref: {emailReference}");
                    }
                    throw;
                }

                return View("ContactUsConfirmation");
            }
            else
            {
                return View("index", contactUs);
            }
        }

        private string GenerateEmailReference(ContactUsViewModel model)
        {
            return $"{model.Name.Substring(0, 3).ToUpper()}{DateTime.UtcNow.Minute}{DateTime.UtcNow.Second}{DateTime.UtcNow.Millisecond}";
        }

    }
}