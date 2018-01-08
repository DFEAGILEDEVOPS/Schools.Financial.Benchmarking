using System.Configuration;
using System.Web.Mvc;

namespace SFB.Web.UI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            var enableAITelemetry = ConfigurationManager.AppSettings["EnableAITelemetry"];
            
            if (enableAITelemetry != null && bool.Parse(enableAITelemetry))
            {
                filters.Add(new ErrorHandler.AiHandleErrorAttribute());
            }
        }
    }
}
