using SFB.Web.UI.Models;
using System;
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

        [HttpPost]
        public ActionResult DataQuerySubmission(DataQueryViewModel dataQuery)
        {
            ViewBag.ModelState = ModelState;
            if (ModelState.IsValid)
            {
                ViewBag.Guid = $"{dataQuery.Reference.Substring(0,3)}-{DateTime.UtcNow.Minute.ToString()}{DateTime.UtcNow.Second.ToString()}{DateTime.UtcNow.Millisecond.ToString()}";
                return View("DataQueryConfirmation");
            }
            else
            {                
                return View("DataQueries", dataQuery);
            }
        }
    }
}