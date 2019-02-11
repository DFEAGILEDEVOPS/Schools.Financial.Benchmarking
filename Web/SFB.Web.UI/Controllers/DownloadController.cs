using System.Threading.Tasks;
using System.Web.Mvc;
using SFB.Web.Common;
using SFB.Web.Domain.Services.DataAccess;

namespace SFB.Web.UI.Controllers
{
    [Authorize]
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
                    ViewBag.Terms = _dataService.GetActiveTermsForAcademies();
                    break;
                default:
                    ViewBag.Terms = _dataService.GetActiveTermsForMaintained();
                    break;
            }

            return View("AcademicYear");
        }

        public async Task<ActionResult> DataSet(EstablishmentType estab, string year)
        {
            if (string.IsNullOrEmpty(year))
            {
                ViewBag.ErrorMessage = "Please select an academic year";
                return AcademicYear(estab);
            }

            ViewBag.Estab = estab;
            ViewBag.Year = year;
            ViewBag.Count = await _dataService.GetEstablishmentRecordCountAsync(year, estab);

            return View();
        }
    }
}