using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SFB.Web.Domain.Services.DataAccess;
using System;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Enums;

namespace SFB.Web.UI.Controllers
{
    public class BenchmarkListController : Controller
    {
        private readonly IContextDataService _contextDataService;
        private readonly IBenchmarkBasketCookieManager _benchmarkBasketCookieManager;

        public BenchmarkListController(IContextDataService contextDataService, IBenchmarkBasketCookieManager benchmarkBasketCookieManager)
        {
            _contextDataService = contextDataService;
            _benchmarkBasketCookieManager = benchmarkBasketCookieManager;
        }

        public ActionResult Index()
        {
            var comparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();

            if (comparisonList.BenchmarkSchools.Count > 1)
            {
                dynamic dynamicBenchmarkSchools = _contextDataService.GetMultipleSchoolsByUrns(comparisonList.BenchmarkSchools.Select(b => Int32.Parse(b.Urn)).ToList());

                comparisonList.BenchmarkSchools = new List<BenchmarkSchoolModel>();

                foreach (var dynamicBenchmarkSchool in dynamicBenchmarkSchools)
                {
                    var school = new SchoolViewModel(dynamicBenchmarkSchool);
                    var benchmarkSchool = new BenchmarkSchoolModel()
                    {
                        Address = school.Address,
                        Name = school.Name,
                        Phase = school.OverallPhase,
                        Type = school.Type,
                        EstabType = school.EstablishmentType.ToString(),
                        Urn = school.Id
                    };

                    comparisonList.BenchmarkSchools.Add(benchmarkSchool);
                }
            }else if (comparisonList.BenchmarkSchools.Count == 1)
            {
                var schoolContextData = _contextDataService.GetSchoolByUrn(Int32.Parse(comparisonList.BenchmarkSchools[0].Urn));

                comparisonList.BenchmarkSchools = new List<BenchmarkSchoolModel>();

                var school = new SchoolViewModel(schoolContextData);
                var benchmarkSchool = new BenchmarkSchoolModel()
                {
                    Address = school.Address,
                    Name = school.Name,
                    Phase = school.OverallPhase,
                    Type = school.Type,
                    EstabType = school.EstablishmentType.ToString(),
                    Urn = school.Id
                };

                comparisonList.BenchmarkSchools.Add(benchmarkSchool);
            }

            return View(comparisonList);
        }

        public PartialViewResult UpdateBenchmarkBasket(int? urn, CookieActions withAction)
        {
            if (urn.HasValue)
            {
                var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolByUrn(urn.GetValueOrDefault()), null);

                _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(withAction,
                    new BenchmarkSchoolModel()
                    {
                        Name = benchmarkSchool.Name,
                        Urn = benchmarkSchool.Id,
                        Type = benchmarkSchool.Type,
                        EstabType = benchmarkSchool.EstablishmentType.ToString()
                    });
            }
            else
            {
                _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(withAction, null);
            }

            return PartialView("Partials/BenchmarkBasketControls", _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie());
        }

    }
}