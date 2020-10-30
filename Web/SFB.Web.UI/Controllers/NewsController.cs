using System.Web.Mvc;
using System.Web.UI;

namespace SFB.Web.UI.Controllers
{
    public class NewsController : Controller
    {
        #if !DEBUG
        [OutputCache (Duration=28800, VaryByParam= "referrer", Location = OutputCacheLocation.Server, NoStore=true)]
        #endif
        public ActionResult Index(string referrer)
        {
            ViewBag.referrer = referrer;
            return View();
        }
    }
}