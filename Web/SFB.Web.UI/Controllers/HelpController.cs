using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SFB.Web.UI.Controllers
{
    public class HelpController : Controller
    {
        public ActionResult Guidance()
        {
            return View();
        }

        public ActionResult InterpretingCharts()
        {
            return View();
        }
    }
}