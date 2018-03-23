using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SFB.Web.Common;
using SFB.Web.Domain.Services.DataAccess;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Constants;

namespace SFB.Web.UI.Controllers
{
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
                case EstablishmentType.Academy:
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