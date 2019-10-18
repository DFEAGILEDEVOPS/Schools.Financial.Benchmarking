using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SFB.Web.ApplicationCore.Services.DataAccess;
using System;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Attributes;

namespace SFB.Web.UI.Controllers
{
    [CustomAuthorize]
    public class BenchmarkListController : Controller
    {
        private readonly IContextDataService _contextDataService;
        private readonly IBenchmarkBasketCookieManager _benchmarkBasketCookieManager;
        private readonly IFinancialDataService _financialDataService;

        public BenchmarkListController(IContextDataService contextDataService, IBenchmarkBasketCookieManager benchmarkBasketCookieManager, IFinancialDataService financialDataService)
        {
            _contextDataService = contextDataService;
            _benchmarkBasketCookieManager = benchmarkBasketCookieManager;
            _financialDataService = financialDataService;
        }

        public ActionResult Index()
        {
            var comparisonList = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();
            
            var benchmarkSchoolDataObjects = _contextDataService.GetMultipleSchoolDataObjectsByUrns(comparisonList.BenchmarkSchools.Select(b => Int32.Parse(b.Urn)).ToList());

            comparisonList.BenchmarkSchools = new List<BenchmarkSchoolModel>();

            if (benchmarkSchoolDataObjects != null)
            {
                foreach (var benchmarkSchoolDataObject in benchmarkSchoolDataObjects)
                {
                    var school = new SchoolViewModel(benchmarkSchoolDataObject);
                    var financialDataModel = _financialDataService.GetSchoolsLatestFinancialDataModel(school.Id, school.EstablishmentType);

                    var benchmarkSchool = new BenchmarkSchoolModel()
                    {
                        Address = school.Address,
                        Name = school.Name,
                        Phase = school.OverallPhase,
                        Type = school.Type,
                        EstabType = school.EstablishmentType.ToString(),
                        Urn = school.Id.ToString(),
                        IsReturnsComplete = financialDataModel.IsReturnsComplete,
                        WorkforceDataPresent = financialDataModel.WorkforceDataPresent
                    };

                    comparisonList.BenchmarkSchools.Add(benchmarkSchool);
                }
            }

            comparisonList.BenchmarkSchools = comparisonList.BenchmarkSchools.OrderBy(s => SanitizeSchoolName(s.Name)).ToList();

            return View(comparisonList);
        }

        public PartialViewResult UpdateBenchmarkBasket(int? urn, CookieActions withAction)
        {
            if (urn.HasValue)
            {
                var schoolDataObject = _contextDataService.GetSchoolDataObjectByUrn(urn.GetValueOrDefault());
                if (schoolDataObject != null)
                {
                    var benchmarkSchool = new SchoolViewModel(schoolDataObject, null);

                    _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(withAction,
                        new BenchmarkSchoolModel()
                        {
                            Name = benchmarkSchool.Name,
                            Urn = benchmarkSchool.Id.ToString(),
                            Type = benchmarkSchool.Type,
                            EstabType = benchmarkSchool.EstablishmentType.ToString()
                        });
                }
            }
            else
            {
                _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(withAction, null);
            }

            return PartialView("Partials/BenchmarkBasketControls", _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie());
        }

        private string SanitizeSchoolName(string name)
        {
            return name                
                .Replace("St ", "Saint")                
                .Replace("De ", "De")
                .Replace("The ", "")
                .Replace("O' ", "O")
                .Replace("-", "")
                .Replace("' ", "");
        }

    }
}