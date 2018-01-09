using System.Configuration;
using System.Web.Mvc;
using SFB.Web.UI.Helpers.Filters;

namespace SFB.Web.UI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new DynamicHeaderMessageAttribute());

            var enableAITelemetry = ConfigurationManager.AppSettings["EnableAITelemetry"];
            
            if (enableAITelemetry != null && bool.Parse(enableAITelemetry))
            {
                filters.Add(new ErrorHandler.AiHandleErrorAttribute());
            }
        }
    }
}
