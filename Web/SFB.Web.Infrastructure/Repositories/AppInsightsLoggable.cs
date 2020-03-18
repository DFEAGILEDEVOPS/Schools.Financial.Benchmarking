using SFB.Web.Infrastructure.Cookies;
using System;
using System.Diagnostics;

namespace SFB.Web.Infrastructure.Repositories
{
    public abstract class AppInsightsLoggable
    {
        private readonly ILogManager _logManager;
        public AppInsightsLoggable(ILogManager logManager)
        {
            _logManager = logManager;
        }

        internal virtual void LogException(Exception exception, string errorMessage)
        {
            Debugger.Break();

            _logManager.LogException(exception, errorMessage);
        }
    }
}
