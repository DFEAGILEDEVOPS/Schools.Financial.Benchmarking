using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFB.Web.ApplicationCore.Services
{
    public interface IEmailSendingService
    {
        Task<string> SendUserEmailAsync(string toAddress, Dictionary<String, dynamic> placeholders);
        Task<string> SendDfEEmailAsync(string toAddress, Dictionary<String, dynamic> placeholders);
        Task<string> SendGetInvolvedEmailAsync(string toAddress, Dictionary<String, dynamic> placeholders);
    }
}