using System.Threading.Tasks;
using System.Web.Mvc;
using SFB.Web.ApplicationCore;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.UI.Attributes;
using SFB.Web.ApplicationCore.Helpers.Enums;

namespace SFB.Web.UI.Controllers
{
    [CustomAuthorize]
    public class DownloadController : Controller
    {
        private readonly IFinancialDataService _dataService;

        public DownloadController(IFinancialDataService dataService)
        {
            _dataService = dataService;
        }

        public ActionResult AcademicYear(EstablishmentType estab)
        {
            ViewBag.Estab = estab;

            switch (estab)
            {
                case EstablishmentType.Academies:
                case EstablishmentType.MAT:
                    ViewBag.Terms = _dataService.GetActiveTermsForAcademiesAsync();
                    break;
                default:
                    ViewBag.Terms = _dataService.GetActiveTermsForMaintainedAsync();
                    break;
            }

            return View("AcademicYear");
        }

        public async Task<ActionResult> DataSet(EstablishmentType estab, string year)
        {
            if (string.IsNullOrEmpty(year))
            {
                ViewBag.ErrorMessage = "Select an academic year";
                return AcademicYear(estab);
            }

            ViewBag.Estab = estab;
            ViewBag.Year = year;
            ViewBag.Count = await _dataService.GetEstablishmentRecordCountAsync(year, estab);

            return View();
        }
    }
}