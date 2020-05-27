using SFB.Web.Infrastructure.Logging;
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

        protected virtual void LogException(Exception exception, string errorMessage)
        {
            Debugger.Break();

            _logManager.LogException(exception, errorMessage);
        }
    }
}
