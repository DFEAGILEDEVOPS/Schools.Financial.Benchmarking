using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFB.Web.UI.Services
{
    public class BenchmarkBasketService : IBenchmarkBasketService
    {
        private IBenchmarkBasketCookieManager _benchmarkBasketCookieManager;
        private IContextDataService _contextDataService;
        private IFinancialDataService _financialDataService;

        public BenchmarkBasketService(IBenchmarkBasketCookieManager benchmarkBasketCookieManager,
            IContextDataService contextDataService,
            IFinancialDataService financialDataService)
        {
            _benchmarkBasketCookieManager = benchmarkBasketCookieManager;
            _contextDataService = contextDataService;
            _financialDataService = financialDataService;
        }

        public void EmptyBenchmarkList()
        {
            _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.RemoveAll, null);
        }

        public void AddDefaultSchoolToBenchmarkList(SchoolViewModel bmSchool)
        {
            try
            {
                _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.Add,
                new BenchmarkSchoolModel()
                {
                    Name = bmSchool.Name,
                    Type = bmSchool.Type,
                    EstabType = bmSchool.EstablishmentType.ToString(),
                    Urn = bmSchool.Id.ToString(),
                    ProgressScore = bmSchool.ProgressScore
                });
            }
            catch (ApplicationException) { }
        }

        public void SetSchoolAsDefaultFromViewModel(SchoolViewModel benchmarkSchool)
        {
            _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.SetDefault,
            new BenchmarkSchoolModel()
            {
                Name = benchmarkSchool.Name,
                Urn = benchmarkSchool.Id.ToString(),
                Type = benchmarkSchool.Type,
                EstabType = benchmarkSchool.EstablishmentType.ToString()
            });
        }

        public async Task SetSchoolAsDefaultFromUrnAsync(int urn)
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

        public void AddSchoolsToBenchmarkListFromComparisonResult(ComparisonResult comparisonResult)
        {
            foreach (var schoolDoc in comparisonResult.BenchmarkSchools)
            {
                var benchmarkSchoolToAdd = new BenchmarkSchoolModel()
                {
                    Name = schoolDoc.SchoolName,
                    Type = schoolDoc.Type,
                    EstabType = schoolDoc.FinanceType,
                    Urn = schoolDoc.URN.ToString()
                };
                _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.Add, benchmarkSchoolToAdd);
            }
        }

        public async Task AddSchoolsToBenchmarkListFromURNsAsync(ComparisonType comparison, List<int> urnList)
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
                if (comparison == ComparisonType.BestInClass)
                {
                    var schoolFinancialData = await _financialDataService.GetSchoolsLatestFinancialDataModelAsync(int.Parse(benchmarkSchoolToAdd.Urn), (EstablishmentType)Enum.Parse(typeof(EstablishmentType), benchmarkSchoolToAdd.EstabType));
                    benchmarkSchoolToAdd.ProgressScore = schoolFinancialData.ProgressScore;
                }
                _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(CookieActions.Add, benchmarkSchoolToAdd);
            }
        }

        public SchoolComparisonListModel ExtractSchoolComparisonListFromCookie()
        {
            return _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();
        }

        public SchoolComparisonListModel ExtractManualComparisonListFromCookie()
        {
            return _benchmarkBasketCookieManager.ExtractManualComparisonListFromCookie();
        }

        public void UpdateSchoolComparisonListCookie(CookieActions withAction, BenchmarkSchoolModel benchmarkSchool)
        {
            _benchmarkBasketCookieManager.UpdateSchoolComparisonListCookie(withAction, benchmarkSchool);
        }

        public void UpdateManualComparisonListCookie(CookieActions withAction, BenchmarkSchoolModel benchmarkSchool)
        {
            _benchmarkBasketCookieManager.UpdateManualComparisonListCookie(withAction, benchmarkSchool);
        }

        public TrustComparisonListModel ExtractTrustComparisonListFromCookie()
        {
            return _benchmarkBasketCookieManager.ExtractTrustComparisonListFromCookie();
        }

        public TrustComparisonListModel UpdateTrustComparisonListCookie(CookieActions withAction, int? companyNo = null, string matName = null)
        {
            return _benchmarkBasketCookieManager.UpdateTrustComparisonListCookie(withAction, companyNo, matName);
        }
    }
}
