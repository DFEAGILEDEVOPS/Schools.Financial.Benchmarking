using System;

namespace SFB.Web.Infrastructure.Cookies
{
    public interface ILogManager
    {
        void LogException(Exception exception, string errorMessage);
    }
}
