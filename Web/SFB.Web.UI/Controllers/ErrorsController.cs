using System.Web.Mvc;

namespace SFB.Web.UI.Controllers
{
    public class ErrorsController : Controller
    {
        // GET: Errors
        public ActionResult NotFound()
        {
            if (this.Request.HttpMethod == "GET"
                && (this.Request.QueryString["aspxerrorpath"] == "/BenchmarkCharts/GenerateFromSimpleCriteria"
                    || this.Request.QueryString["aspxerrorpath"] == "/BenchmarkCharts/GenerateFromAdvancedCriteria"))
            {
                return View("InvalidRequest");
            }

            return View();
        }

        public ActionResult InternalServerError()
        {
            return View();
        }
    }
}