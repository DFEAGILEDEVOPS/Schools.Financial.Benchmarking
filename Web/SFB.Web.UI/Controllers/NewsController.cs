using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SFB.Web.UI.Controllers
{
    public class NewsController : Controller
    {
        // GET: News
        public ActionResult Index(string referrer)
        {
            ViewBag.referrer = referrer;
            return View();
        }
    }
}