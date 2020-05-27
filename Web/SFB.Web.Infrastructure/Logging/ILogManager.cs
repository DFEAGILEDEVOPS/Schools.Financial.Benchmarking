using System;

namespace SFB.Web.Infrastructure.Logging
{
    public interface ILogManager
    {
        void LogException(Exception exception, string errorMessage);
    }
}
