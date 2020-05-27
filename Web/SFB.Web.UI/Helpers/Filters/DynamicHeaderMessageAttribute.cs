using System.Configuration;
using System.Web.Mvc;

namespace SFB.Web.UI.Helpers.Filters
{
    public class DynamicHeaderMessageAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.Result is ViewResult)
            {
                (filterContext.Result as ViewResult).ViewBag.DynamicHeaderContent =
                    ConfigurationManager.AppSettings["DynamicHeaderContent"];
            }

        }
    }
}