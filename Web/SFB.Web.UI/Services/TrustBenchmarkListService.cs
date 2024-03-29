﻿using SFB.Web.ApplicationCore.Helpers;
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
    public class TrustBenchmarkListService : BenchmarkBasketCookieManager, ITrustBenchmarkListService
    {
        private IFinancialDataService _financialDataService;

        public TrustBenchmarkListService(IFinancialDataService financialDataService)
        {
            _financialDataService = financialDataService;
        }

        public void AddDefaultTrustToBenchmarkList()
        {
            UpdateTrustComparisonListCookie(CookieActions.AddDefaultToList);
        }

        public TrustComparisonListModel RemoveTrustFromBenchmarkList(int companyNo)
        {
            return UpdateTrustComparisonListCookie(CookieActions.Remove, companyNo);
        }

        public TrustComparisonListModel ClearTrustBenchmarkList()
        {
            return UpdateTrustComparisonListCookie(CookieActions.RemoveAll);
        }

        public TrustComparisonListModel GetTrustBenchmarkList()
        {
            return ExtractTrustComparisonListFromCookie();
        }

        public TrustComparisonListModel AddTrustToBenchmarkList(int companyNumber, string trustOrCompanyName)
        {
            return UpdateTrustComparisonListCookie(CookieActions.Add, companyNumber, trustOrCompanyName);
        }

        public TrustComparisonListModel TryAddTrustToBenchmarkList(int companyNumber, string trustOrCompanyName)
        {
            try
            {
                return UpdateTrustComparisonListCookie(CookieActions.Add, companyNumber, trustOrCompanyName);
            }
            catch (ApplicationException)
            {
                return null;
            }//Ignore double add attempts
        }

        public async Task<TrustViewModel> SetTrustAsDefaultAsync(int companyNo)
        {
            var latestTerm = await LatestMATTermAsync();
            var trustFinancialDataObject = await _financialDataService.GetTrustFinancialDataObjectByCompanyNoAsync(companyNo, latestTerm, MatFinancingType.TrustAndAcademies);

            TryAddTrustToBenchmarkList(companyNo, trustFinancialDataObject.TrustOrCompanyName);

            UpdateTrustComparisonListCookie(CookieActions.SetDefault, companyNo, trustFinancialDataObject.TrustOrCompanyName);
            
            var vm = new TrustViewModel(companyNo, trustFinancialDataObject.TrustOrCompanyName);
            vm.HistoricalFinancialDataModels = new List<FinancialDataModel>
            {
                new FinancialDataModel(companyNo.ToString(), latestTerm, trustFinancialDataObject, EstablishmentType.MAT)
            };

            return vm;
        }

        private async Task<string> LatestMATTermAsync()
        {
            var latestYear = await _financialDataService.GetLatestDataYearPerEstabTypeAsync(EstablishmentType.MAT);
            return SchoolFormatHelpers.FinancialTermFormatAcademies(latestYear);
        }
    }
 }
