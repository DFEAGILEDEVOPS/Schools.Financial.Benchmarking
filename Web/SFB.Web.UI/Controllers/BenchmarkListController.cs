using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SFB.Web.Domain.Services.DataAccess;
using System;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.Common.DataObjects;
using SFB.Web.Common;

namespace SFB.Web.UI.Controllers
{
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

            if (comparisonList.BenchmarkSchools.Count > 1)
            {
                var benchmarkSchoolDataObjects = _contextDataService.GetMultipleSchoolDataObjectsByUrns(comparisonList.BenchmarkSchools.Select(b => Int32.Parse(b.Urn)).ToList());

                comparisonList.BenchmarkSchools = new List<BenchmarkSchoolModel>();

                foreach (var benchmarkSchoolDataObject in benchmarkSchoolDataObjects)
                {
                    var school = new SchoolViewModel(benchmarkSchoolDataObject);
                    var latestYear = _financialDataService.GetLatestDataYearPerEstabType(school.EstablishmentType);
                    var term = FormatHelpers.FinancialTermFormatAcademies(latestYear);
                    var financialDataDocument = _financialDataService.GetSchoolFinancialDataObject(school.Id, term, school.EstablishmentType);

                    var benchmarkSchool = new BenchmarkSchoolModel()
                    {
                        Address = school.Address,
                        Name = school.Name,
                        Phase = school.OverallPhase,
                        Type = school.Type,
                        EstabType = school.EstablishmentType.ToString(),
                        Urn = school.Id.ToString(),
                        IsReturnsComplete = financialDataDocument.PeriodCoveredByReturn == 12,
                        WorkforceDataPresent = financialDataDocument.WorkforcePresent
                    };

                    comparisonList.BenchmarkSchools.Add(benchmarkSchool);
                }
            }else if (comparisonList.BenchmarkSchools.Count == 1)
            {
                var schoolContextData = _contextDataService.GetSchoolDataObjectByUrn(Int32.Parse(comparisonList.BenchmarkSchools[0].Urn));
                var school = new SchoolViewModel(schoolContextData);
                var latestYear = _financialDataService.GetLatestDataYearPerEstabType(school.EstablishmentType);
                var term = FormatHelpers.FinancialTermFormatAcademies(latestYear);
                var financialDataDocument = _financialDataService.GetSchoolFinancialDataObject(school.Id, term, school.EstablishmentType); comparisonList.BenchmarkSchools = new List<BenchmarkSchoolModel>();

                var benchmarkSchool = new BenchmarkSchoolModel()
                {
                    Address = school.Address,
                    Name = school.Name,
                    Phase = school.OverallPhase,
                    Type = school.Type,
                    EstabType = school.EstablishmentType.ToString(),
                    Urn = school.Id.ToString(),
                    IsReturnsComplete = financialDataDocument.PeriodCoveredByReturn == 12,
                    WorkforceDataPresent = financialDataDocument.WorkforcePresent
                };

                comparisonList.BenchmarkSchools.Add(benchmarkSchool);
            }

            comparisonList.BenchmarkSchools = comparisonList.BenchmarkSchools.OrderBy(s => SanitizeSchoolName(s.Name)).ToList();

            return View(comparisonList);
        }

        public PartialViewResult UpdateBenchmarkBasket(int? urn, CookieActions withAction)
        {
            if (urn.HasValue)
            {
                var benchmarkSchool = new SchoolViewModel(_contextDataService.GetSchoolDataObjectByUrn(urn.GetValueOrDefault()), null);

                _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(withAction,
                    new BenchmarkSchoolModel()
                    {
                        Name = benchmarkSchool.Name,
                        Urn = benchmarkSchool.Id.ToString(),
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