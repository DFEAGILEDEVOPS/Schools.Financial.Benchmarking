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

        public ActionResult DataSources()
        {
            return View();
        }
    }
}