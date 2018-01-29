using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
                    "The schools financial benchmarking tool will be updated in the summer term with academy financial data for 2016/17. Local authority maintained school financial data will be updated in early 2019.";
            }

        }
    }
}