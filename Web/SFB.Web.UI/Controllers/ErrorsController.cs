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
                    || this.Request.QueryString["aspxerrorpath"] == "/BenchmarkCharts/GenerateFromAdvancedCriteria"
                    || this.Request.QueryString["aspxerrorpath"] == "/BenchmarkCharts/GenerateNewFromAdvancedCriteria"))
            {
                return View("InvalidRequest");
            }

            return View();
        }

        public ActionResult InvalidRequest()
        {
            return View();
        }

        public ActionResult InternalServerError()
        {
            return View();
        }
    }
}