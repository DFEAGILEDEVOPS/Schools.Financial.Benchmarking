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
    public class SchoolBenchmarkListService : BenchmarkBasketCookieManager, ISchoolBenchmarkListService
    {
        private readonly IContextDataService _contextDataService;
        private readonly IFinancialDataService _financialDataService;

        public SchoolBenchmarkListService(IContextDataService contextDataService, IFinancialDataService financialDataService)
        {
            _contextDataService = contextDataService;
            _financialDataService = financialDataService;
        }

        public async Task TryAddSchoolToBenchmarkListAsync(int urn)
        {
            var schoolDataObject = await _contextDataService.GetSchoolDataObjectByUrnAsync(urn);
            if (schoolDataObject != null)
            {
                var benchmarkSchool = new SchoolViewModel(schoolDataObject, null);
                try { 
                    AddSchoolToBenchmarkList(benchmarkSchool);
                }
                catch (ApplicationException) { }//ignore duplicate add
            }
        }
        public async Task AddSchoolToBenchmarkListAsync(int urn)
        {
            var contextData = await _contextDataService.GetSchoolDataObjectByUrnAsync(urn);
            if (contextData != null)
            {
                if (contextData.IsFederation)
                {
                    var benchmarkFederation = new FederationViewModel(contextData, null);
                    AddFederationToBenchmarkList(benchmarkFederation);
                }
                else
                {
                    var benchmarkSchool = new SchoolViewModel(contextData, null);
                    AddSchoolToBenchmarkList(benchmarkSchool);
                }
            }            
        }

        public void AddSchoolToBenchmarkList(BenchmarkSchoolModel bmSchool)
        {
            UpdateSchoolComparisonListCookie(CookieActions.Add, bmSchool);
        }

        public void TryAddSchoolToBenchmarkList(BenchmarkSchoolModel bmSchool)
        {
            try
            {
                UpdateSchoolComparisonListCookie(CookieActions.Add, bmSchool);
            }
            catch (ApplicationException) { }//ignore duplicate add
        }

        public void AddSchoolToBenchmarkList(SchoolViewModel schoolVM)
        {
            AddSchoolToBenchmarkList(new BenchmarkSchoolModel(schoolVM));                            
        }

        public void AddFederationToBenchmarkList(FederationViewModel fedVM)
        {
            AddSchoolToBenchmarkList(new BenchmarkSchoolModel(fedVM));
        }

        public void TryAddSchoolToBenchmarkList(SchoolViewModel schoolVM)
        {
            try
            {
                AddSchoolToBenchmarkList(new BenchmarkSchoolModel(schoolVM));
            }
            catch (ApplicationException) { }//ignore duplicate add
        }

        public void AddSchoolsToBenchmarkList(ComparisonResult comparisonResult)
        {
            foreach (var schoolFinance in comparisonResult.BenchmarkSchools)
            {
                TryAddSchoolToBenchmarkList(new BenchmarkSchoolModel(schoolFinance));
            }
        }

        public async Task AddSchoolsToBenchmarkListAsync(ComparisonType comparison, List<int> urnList)
        {
            var benchmarkSchoolDataObjects = await _contextDataService.GetMultipleSchoolDataObjectsByUrnsAsync(urnList);

            foreach (var schoolContextData in benchmarkSchoolDataObjects)
            {
                var benchmarkSchoolToAdd = new BenchmarkSchoolModel(schoolContextData);

                if (comparison == ComparisonType.BestInClass)
                {
                    var schoolFinancialData = await _financialDataService.GetSchoolsLatestFinancialDataModelAsync(int.Parse(benchmarkSchoolToAdd.Urn), (EstablishmentType)Enum.Parse(typeof(EstablishmentType), benchmarkSchoolToAdd.EstabType));
                    benchmarkSchoolToAdd.ProgressScore = schoolFinancialData.ProgressScore;
                }
                TryAddSchoolToBenchmarkList(benchmarkSchoolToAdd);
            }
        }

        public void SetSchoolAsDefault(SchoolViewModel benchmarkSchool)
        {
            UpdateSchoolComparisonListCookie(CookieActions.SetDefault, new BenchmarkSchoolModel(benchmarkSchool));
        }

        public void SetFederationAsDefault(FederationViewModel benchmarkSchool)
        {
            UpdateSchoolComparisonListCookie(CookieActions.SetDefault, new BenchmarkSchoolModel(benchmarkSchool));
        }

        public void SetSchoolAsDefault(BenchmarkSchoolModel benchmarkSchool)
        {
            UpdateSchoolComparisonListCookie(CookieActions.SetDefault, benchmarkSchool);            
        }

        public async Task SetSchoolAsDefaultAsync(int urn)
        {
            var benchmarkSchoolDataObject = await _contextDataService.GetSchoolDataObjectByUrnAsync(urn);

            var defaultBenchmarkSchool = new BenchmarkSchoolModel(benchmarkSchoolDataObject);

            try
            {
                AddSchoolToBenchmarkList(defaultBenchmarkSchool);
            }
            catch { }
            UpdateSchoolComparisonListCookie(CookieActions.SetDefault, defaultBenchmarkSchool);
        }

        public SchoolComparisonListModel GetSchoolBenchmarkList()
        {
            return ExtractSchoolComparisonListFromCookie();
        }

        public void ClearSchoolBenchmarkList()
        {
            UpdateSchoolComparisonListCookie(CookieActions.RemoveAll);
        }

        public void UnsetDefaultSchool()
        {
            UpdateSchoolComparisonListCookie(CookieActions.UnsetDefault);
        }

        public async Task RemoveSchoolFromBenchmarkListAsync(int urn)
        {
            var benchmarkSchool = new SchoolViewModel(await _contextDataService.GetSchoolDataObjectByUrnAsync(urn), null);

            UpdateSchoolComparisonListCookie(CookieActions.Remove, new BenchmarkSchoolModel(benchmarkSchool));
        }
    }
 }
