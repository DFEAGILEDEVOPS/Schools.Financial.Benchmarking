using Notify.Models.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFB.Web.UI.Services
{
    public interface IEmailSendingService
    {
        Task<EmailNotificationResponse> SendUserEmailAsync(string toAddress, Dictionary<String, dynamic> placeholders);
        Task<EmailNotificationResponse> SendDfEEmailAsync(string toAddress, Dictionary<String, dynamic> placeholders);
    }
}