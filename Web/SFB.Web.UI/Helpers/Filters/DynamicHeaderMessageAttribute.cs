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
                    "This is a message to users about the new release of the latest data for schools and trusts. " +
                    "It might contain a <a href=\"http://www.gov.uk\">link</a> to some information but doesn't have to.";
            }

        }
    }
}