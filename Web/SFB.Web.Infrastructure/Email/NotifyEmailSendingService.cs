using Notify.Client;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
using SFB.Web.ApplicationCore.Services;

namespace SFB.Web.Infrastructure.Email
{
    public class NotifyEmailSendingService : IEmailSendingService
    {
        public async Task<string> SendUserEmailAsync(string toAddress, Dictionary<string, dynamic> placeholders)
        {
            return await SendEmail(toAddress, ConfigurationManager.AppSettings["UserEmailTemplateId"], placeholders);
        }

        public async Task<string> SendDfEEmailAsync(string toAddress, Dictionary<string, dynamic> placeholders)
        {
            return await SendEmail(toAddress, ConfigurationManager.AppSettings["DfEEmailTemplateId"], placeholders);
        }

        public async Task<string> SendGetInvolvedEmailAsync(string toAddress, Dictionary<string, dynamic> placeholders)
        {
            return await SendEmail(toAddress, ConfigurationManager.AppSettings["RecruitmentEmailTemplateId"], placeholders);
        }

        private async Task<string> SendEmail(string toAddress, string templateId, Dictionary<string, dynamic> placeholders)
        {
            var apiKey = ConfigurationManager.AppSettings["NotifyAPIKey"];            
            var client = new NotificationClient(apiKey);

            var response = await client.SendEmailAsync(
                                emailAddress: toAddress,
                                templateId: templateId,
                                personalisation: placeholders
                                );

            return response.reference;
        }
    }
}
