using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SFB.Web.Domain.Services.DataAccess;
using System;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Enums;
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
                dynamic dynamicBenchmarkSchools = _contextDataService.GetMultipleSchoolsByUrns(comparisonList.BenchmarkSchools.Select(b => Int32.Parse(b.Urn)).ToList());

                comparisonList.BenchmarkSchools = new List<BenchmarkSchoolModel>();

                foreach (var dynamicBenchmarkSchool in dynamicBenchmarkSchools)
                {
                    var latestYear = _financialDataService.GetLatestDataYearPerEstabType((EstablishmentType)Enum.Parse(typeof(EstablishmentType), dynamicBenchmarkSchool.FinanceType));
                    var term = FormatHelpers.FinancialTermFormatAcademies(latestYear);
                    var financialDataDocument = _financialDataService.GetSchoolDataDocument(dynamicBenchmarkSchool.GetPropertyValue<int>("URN"), term, (EstablishmentType)Enum.Parse(typeof(EstablishmentType), dynamicBenchmarkSchool.FinanceType));

                    var school = new SchoolViewModel(dynamicBenchmarkSchool);
                    var benchmarkSchool = new BenchmarkSchoolModel()
                    {
                        Address = school.Address,
                        Name = school.Name,
                        Phase = school.OverallPhase,
                        Type = school.Type,
                        EstabType = school.EstablishmentType.ToString(),
                        Urn = school.Id,
                        IsReturnsComplete = financialDataDocument.GetPropertyValue<int>("Period covered by return") == 12,
                        WorkforceDataPresent = financialDataDocument.GetPropertyValue<bool>("WorkforcePresent")
                    };

                    comparisonList.BenchmarkSchools.Add(benchmarkSchool);
                }
            }else if (comparisonList.BenchmarkSchools.Count == 1)
            {
                var schoolContextData = _contextDataService.GetSchoolByUrn(Int32.Parse(comparisonList.BenchmarkSchools[0].Urn));

                var latestYear = _financialDataService.GetLatestDataYearPerEstabType((EstablishmentType)Enum.Parse(typeof(EstablishmentType), schoolContextData.FinanceType));
                var term = FormatHelpers.FinancialTermFormatAcademies(latestYear);
                var financialDataDocument = _financialDataService.GetSchoolDataDocument(schoolContextData.GetPropertyValue<int>("URN"), term, (EstablishmentType)Enum.Parse(typeof(EstablishmentType), schoolContextData.FinanceType));

                comparisonList.BenchmarkSchools = new List<BenchmarkSchoolModel>();

                var school = new SchoolViewModel(schoolContextData);
                var benchmarkSchool = new BenchmarkSchoolModel()
                {
                    Address = school.Address,
                    Name = school.Name,
                    Phase = school.OverallPhase,
                    Type = school.Type,
                    EstabType = school.EstablishmentType.ToString(),
                    Urn = school.Id,
                    IsReturnsComplete = financialDataDocument.GetPropertyValue<int>("Period covered by return") == 12,
                    WorkforceDataPresent = financialDataDocument.GetPropertyValue<bool>("WorkforcePresent")
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