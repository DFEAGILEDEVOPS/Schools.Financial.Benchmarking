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
                    "The schools financial benchmarking tool will be updated later this month (January 2018) with 2016 - 17 local authority maintained school financial data. Academy financial data for 2016 / 17 will be uploaded in the summer term.";
            }

        }
    }
}