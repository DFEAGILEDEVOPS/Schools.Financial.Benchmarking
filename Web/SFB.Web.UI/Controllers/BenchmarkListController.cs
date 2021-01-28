using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SFB.Web.ApplicationCore.Services.DataAccess;
using System;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Attributes;
using SFB.Web.UI.Services;
using System.Threading.Tasks;

namespace SFB.Web.UI.Controllers
{
    [CustomAuthorize]
    public class BenchmarkListController : Controller
    {
        private readonly IContextDataService _contextDataService;
        private readonly IBenchmarkBasketService _benchmarkBasketService;
        private readonly IFinancialDataService _financialDataService;

        public BenchmarkListController(IContextDataService contextDataService, IBenchmarkBasketService benchmarkBasketService, IFinancialDataService financialDataService)
        {
            _contextDataService = contextDataService;
            _benchmarkBasketService = benchmarkBasketService;
            _financialDataService = financialDataService;
        }

        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            var comparisonList = _benchmarkBasketService.GetSchoolBenchmarkList();
            
            var benchmarkSchoolDataObjects = await _contextDataService.GetMultipleSchoolDataObjectsByUrnsAsync(comparisonList.BenchmarkSchools.Select(b => Int32.Parse(b.Urn)).ToList());

            comparisonList.BenchmarkSchools = new List<BenchmarkSchoolModel>();

            if (benchmarkSchoolDataObjects != null)
            {
                foreach (var benchmarkSchoolDataObject in benchmarkSchoolDataObjects)
                {
                    var school = new SchoolViewModel(benchmarkSchoolDataObject);
                    var financialDataModel = await _financialDataService.GetSchoolsLatestFinancialDataModelAsync(school.Id, school.EstablishmentType);

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

        public async Task<PartialViewResult> UpdateBenchmarkBasketAsync(int? urn, CookieActions withAction)
        {
            if (urn.HasValue)
            {
                switch (withAction)
                {
                    case CookieActions.SetDefault:
                        await _benchmarkBasketService.SetSchoolAsDefaultAsync(urn.GetValueOrDefault());
                        break;
                    case CookieActions.Add:
                        await _benchmarkBasketService.AddSchoolToBenchmarkListAsync(urn.GetValueOrDefault());
                        break;
                    case CookieActions.Remove:
                        await _benchmarkBasketService.RemoveSchoolFromBenchmarkListAsync(urn.GetValueOrDefault());
                        break;
                    case CookieActions.RemoveAll:
                        _benchmarkBasketService.ClearSchoolBenchmarkList();
                        break;
                    case CookieActions.UnsetDefault:
                        _benchmarkBasketService.UnsetDefaultSchool();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                _benchmarkBasketService.ClearSchoolBenchmarkList();
            }

            return PartialView("Partials/BenchmarkBasketControls", _benchmarkBasketService.GetSchoolBenchmarkList());
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