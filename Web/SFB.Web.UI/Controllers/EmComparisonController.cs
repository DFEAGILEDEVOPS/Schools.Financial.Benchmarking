using SFB.Web.ApplicationCore.Entities;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFB.Web.UI.Controllers
{
    public class EmComparisonController : Controller
    {
        private readonly IBenchmarkBasketCookieManager _benchmarkBasketCookieManager;
        private readonly IContextDataService _contextDataService;
        private readonly IEfficiencyMetricDataService _efficiencyMetricDataService;
        public EmComparisonController(IBenchmarkBasketCookieManager benchmarkBasketCookieManager, IContextDataService contextDataService, IEfficiencyMetricDataService efficiencyMetricDataService)
        {
            _benchmarkBasketCookieManager = benchmarkBasketCookieManager;
            _contextDataService = contextDataService;
            _efficiencyMetricDataService = efficiencyMetricDataService;
        }

        // GET: EmComparison
        public async Task<ActionResult> Index(int urn, ComparisonType comparisonType)
        {
            var defaultSchool = await _efficiencyMetricDataService.GetSchoolDataObjectByUrnAsync(urn);
            var neighbourSchools = (await _efficiencyMetricDataService.GetSchoolDataObjectByUrnAsync(urn)).NeighbourRecords;
            neighbourSchools.Add(new EfficiencyMetricNeighbourListItemObject(defaultSchool.Urn, defaultSchool.Efficiencydecileingroup));
            var topNeighbourSchoolURNs = neighbourSchools.OrderBy(s => s.Rank).Take(15).Select(n => n.URN).ToList();

            EmptyBasket();

            await AddSchoolDataObjectsToBasketAsync(topNeighbourSchoolURNs);

            await SetDefaultSchoolInBasketAsync(urn);

            return Redirect($"BenchmarkCharts");
        }

        private void EmptyBasket()
        {
            _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.RemoveAll, null);
        }

        private async Task AddSchoolDataObjectsToBasketAsync(List<int> urnList)
        {
            var benchmarkSchoolDataObjects = await _contextDataService.GetMultipleSchoolDataObjectsByUrnsAsync(urnList);

            foreach (var schoolContextData in benchmarkSchoolDataObjects)
            {
                var benchmarkSchoolToAdd = new BenchmarkSchoolModel()
                {
                    Name = schoolContextData.EstablishmentName,
                    Type = schoolContextData.TypeOfEstablishment,
                    EstabType = schoolContextData.FinanceType,
                    Urn = schoolContextData.URN.ToString()
                };
                _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.Add, benchmarkSchoolToAdd);
            }
        }  

        private async Task SetDefaultSchoolInBasketAsync(int urn)
        {
            var benchmarkSchoolDataObject = await _contextDataService.GetSchoolDataObjectByUrnAsync(urn);

            var defaultBenchmarkSchool = new BenchmarkSchoolModel()
            {
                Name = benchmarkSchoolDataObject.EstablishmentName,
                Type = benchmarkSchoolDataObject.TypeOfEstablishment,
                EstabType = benchmarkSchoolDataObject.FinanceType,
                Urn = benchmarkSchoolDataObject.URN.ToString()
            };

            try
            {
                _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.Add, defaultBenchmarkSchool);
            }
            catch { }
            _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.SetDefault, defaultBenchmarkSchool);
        }


    }
}