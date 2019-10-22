using Notify.Client;
using System.Collections.Generic;
using System.Threading.Tasks;
using Notify.Models.Responses;
using System.Configuration;

namespace SFB.Web.UI.Services
{
    public class NotifyEmailSendingService : IEmailSendingService
    {
        public async Task<EmailNotificationResponse> SendUserEmailAsync(string toAddress, Dictionary<string, dynamic> placeholders)
        {
            return await SendEmail(toAddress, ConfigurationManager.AppSettings["UserEmailTemplateId"], placeholders);
        }

        public async Task<EmailNotificationResponse> SendDfEEmailAsync(string toAddress, Dictionary<string, dynamic> placeholders)
        {
            return await SendEmail(toAddress, ConfigurationManager.AppSettings["DfEEmailTemplateId"], placeholders);
        }

        private async Task<EmailNotificationResponse> SendEmail(string toAddress, string templateId, Dictionary<string, dynamic> placeholders)
        {
            var apiKey = ConfigurationManager.AppSettings["NotifyAPIKey"];            
            var client = new NotificationClient(apiKey);

            var response = await client.SendEmailAsync(
                                emailAddress: toAddress,
                                templateId: templateId,
                                personalisation: placeholders
                                );

            return response;
        }
    }
}
