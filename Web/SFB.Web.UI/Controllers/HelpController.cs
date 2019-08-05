using SFB.Web.UI.Models;
using System.Linq;
using System.Text;
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

        public ActionResult DataQueries()
        {
            ViewBag.ModelState = ModelState;
            return View(new DataQueryViewModel());
        }

        public ActionResult DataQuerySubmission(DataQueryViewModel dataQuery)
        {
            ViewBag.ModelState = ModelState;
            if (ModelState.IsValid)
            {
                //Todo: Generate reference number and send emails.
                return View("DataQueryConfirmation");
            }
            else
            {                
                return View("DataQueries", dataQuery);
            }
        }
    }
}