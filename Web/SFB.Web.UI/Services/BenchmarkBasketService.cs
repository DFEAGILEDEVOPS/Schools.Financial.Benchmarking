using SFB.Web.ApplicationCore.Helpers;
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
    public class BenchmarkBasketService : BenchmarkBasketCookieManager, IBenchmarkBasketService
    {
        private IContextDataService _contextDataService;
        private IFinancialDataService _financialDataService;

        private async Task<string> LatestMATTermAsync()
        {
            var latestYear = await _financialDataService.GetLatestDataYearPerEstabTypeAsync(EstablishmentType.MAT);
            return SchoolFormatHelpers.FinancialTermFormatAcademies(latestYear);
        }

        public BenchmarkBasketService(IContextDataService contextDataService, IFinancialDataService financialDataService)
        {
            _contextDataService = contextDataService;
            _financialDataService = financialDataService;
        }

        public async Task AddSchoolToBenchmarkListAsync(int urn)
        {
            var schoolDataObject = await _contextDataService.GetSchoolDataObjectByUrnAsync(urn);
            if (schoolDataObject != null)
            {
                var benchmarkSchool = new SchoolViewModel(schoolDataObject, null);
                AddSchoolToBenchmarkList(benchmarkSchool);
            }            
        }

        public void AddSchoolToBenchmarkList(SchoolViewModel bmSchool)
        {
            try
            {
                UpdateSchoolComparisonListCookie(CookieActions.Add,
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

        public void AddSchoolToManualBenchmarkList(SchoolViewModel bmSchool)
        {
            try
            {
                UpdateManualComparisonListCookie(CookieActions.Add,
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

        public async Task AddSchoolToManualBenchmarkListAsync(int urn)
        {
            var benchmarkSchool = new SchoolViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(urn), null);
            AddSchoolToManualBenchmarkList(benchmarkSchool);
        }

        public async Task SetSchoolAsDefaultInManualComparisonList(int urn)
        {
            var bmSchool = new SchoolViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(urn), null);
            try
            {
                UpdateManualComparisonListCookie(CookieActions.SetDefault,
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

        public void AddSchoolsToBenchmarkList(ComparisonResult comparisonResult)
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
                UpdateSchoolComparisonListCookie(CookieActions.Add, benchmarkSchoolToAdd);
            }
        }

        public async Task AddSchoolsToBenchmarkListAsync(ComparisonType comparison, List<int> urnList)
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
                UpdateSchoolComparisonListCookie(CookieActions.Add, benchmarkSchoolToAdd);
            }
        }

        public void SetSchoolAsDefaultInManualComparisonList(SchoolComparisonListModel schoolComparisonList)
        {
            UpdateManualComparisonListCookie(CookieActions.SetDefault, new BenchmarkSchoolModel()
            {
                Name = schoolComparisonList.HomeSchoolName,
                Urn = schoolComparisonList.HomeSchoolUrn,
                Type = schoolComparisonList.HomeSchoolType,
                EstabType = schoolComparisonList.HomeSchoolFinancialType
            });
        }

        public void SetSchoolAsDefault(SchoolViewModel benchmarkSchool)
        {
            UpdateSchoolComparisonListCookie(CookieActions.SetDefault,
            new BenchmarkSchoolModel()
            {
                Name = benchmarkSchool.Name,
                Urn = benchmarkSchool.Id.ToString(),
                Type = benchmarkSchool.Type,
                EstabType = benchmarkSchool.EstablishmentType.ToString()
            });
        }

        public void SetSchoolAsDefault(BenchmarkSchoolModel benchmarkSchool)
        {
            UpdateSchoolComparisonListCookie(CookieActions.SetDefault, benchmarkSchool);            
        }

        public async Task SetTrustAsDefaultAsync(int companyNo)
        {
            var latestTerm = await LatestMATTermAsync();
            var trustFinancialDataObject = await _financialDataService.GetTrustFinancialDataObjectByCompanyNoAsync(companyNo, latestTerm, MatFinancingType.TrustOnly);

            try
            {
                UpdateTrustComparisonListCookie(CookieActions.Add, companyNo, trustFinancialDataObject.TrustOrCompanyName);
            }
            catch { }
            UpdateTrustComparisonListCookie(CookieActions.SetDefault, companyNo, trustFinancialDataObject.TrustOrCompanyName);
        }

        public async Task SetSchoolAsDefaultAsync(int urn)
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
                UpdateSchoolComparisonListCookie(CookieActions.Add, defaultBenchmarkSchool);
            }
            catch { }
            UpdateSchoolComparisonListCookie(CookieActions.SetDefault, defaultBenchmarkSchool);
        }

        public SchoolComparisonListModel GetSchoolBenchmarkList()
        {
            return ExtractSchoolComparisonListFromCookie();
        }

        public SchoolComparisonListModel GetManualBenchmarkList()
        {
            return ExtractManualComparisonListFromCookie();
        }

        public TrustComparisonListModel GetTrustBenchmarkList()
        {
            return ExtractTrustComparisonListFromCookie();
        }

        public TrustComparisonListModel AddTrustToBenchmarkList(int companyNumber, string trustOrCompanyName)
        {
            return UpdateTrustComparisonListCookie(CookieActions.Add, companyNumber, trustOrCompanyName);
        }

        public TrustComparisonListModel ClearTrustBenchmarkList()
        {
            return UpdateTrustComparisonListCookie(CookieActions.RemoveAll);
        }

        public void ClearSchoolBenchmarkList()
        {
            UpdateSchoolComparisonListCookie(CookieActions.RemoveAll);
        }

        public void ClearManualBenchmarkList()
        {
            UpdateManualComparisonListCookie(CookieActions.RemoveAll);
        }

        public void AddSchoolToBenchmarkList(BenchmarkSchoolModel bmSchool)
        {
            UpdateSchoolComparisonListCookie(CookieActions.Add, bmSchool);
        }

        public void UnsetDefaultSchool()
        {
            UpdateSchoolComparisonListCookie(CookieActions.UnsetDefault);
        }

        public void UnsetDefaultSchoolInManualBenchmarkList()
        {
            UpdateManualComparisonListCookie(CookieActions.UnsetDefault);
        }

        public async Task RemoveSchoolFromBenchmarkListAsync(int urn)
        {
            var benchmarkSchool = new SchoolViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(urn), null);

            UpdateSchoolComparisonListCookie(CookieActions.Remove, new BenchmarkSchoolModel()
            {
                Name = benchmarkSchool.Name,
                Urn = benchmarkSchool.Id.ToString(),
                Type = benchmarkSchool.Type,
                EstabType = benchmarkSchool.EstablishmentType.ToString()
            });
        }

        public async Task RemoveSchoolFromManualBenchmarkListAsync(int urn)
        {
            var benchmarkSchool = new SchoolViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(urn), null);

            UpdateManualComparisonListCookie(CookieActions.Remove, new BenchmarkSchoolModel()
            {
                Name = benchmarkSchool.Name,
                Urn = benchmarkSchool.Id.ToString(),
                Type = benchmarkSchool.Type,
                EstabType = benchmarkSchool.EstablishmentType.ToString()
            });
        }

        public void AddDefaultTrustToBenchmarkList()
        {
            UpdateTrustComparisonListCookie(CookieActions.AddDefaultToList);
        }

        public TrustComparisonListModel RemoveTrustFromBenchmarkList(int companyNo)
        {
            return UpdateTrustComparisonListCookie(CookieActions.Remove, companyNo);
        }
    }
}
