﻿using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Services;
using System.Text;
using Microsoft.Ajax.Utilities;
using SFB.Web.ApplicationCore;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.ApplicationCore.Services.Search;
using SFB.Web.ApplicationCore.Entities;
using System;
using SFB.Web.UI.Attributes;
using System.Web.Routing;
using SFB.Web.ApplicationCore.Helpers.Enums;

namespace SFB.Web.UI.Controllers
{
    [CustomAuthorize]
    public class TrustController : Controller
    {
        private readonly ITrustSearchService _trustSearchService;
        private readonly IFinancialDataService _financialDataService;
        private readonly IHistoricalChartBuilder _historicalChartBuilder;
        private readonly IFinancialCalculationsService _fcService;
        private readonly IDownloadCSVBuilder _csvBuilder;
        private readonly IBenchmarkBasketCookieManager _benchmarkBasketCookieManager;

        public TrustController(IHistoricalChartBuilder historicalChartBuilder, IFinancialDataService financialDataService, 
            IFinancialCalculationsService fcService, ITrustSearchService trustSearchService, IDownloadCSVBuilder csvBuilder,
            IBenchmarkBasketCookieManager benchmarkBasketCookieManager)
        {
            _historicalChartBuilder = historicalChartBuilder;
            _financialDataService = financialDataService;
            _fcService = fcService;
            _trustSearchService = trustSearchService;
            _csvBuilder = csvBuilder;
            _benchmarkBasketCookieManager = benchmarkBasketCookieManager;
        }

        public async Task<ActionResult> Index(int companyNo, UnitType unit = UnitType.AbsoluteMoney, RevenueGroupType tab = RevenueGroupType.Expenditure, MatFinancingType financing = MatFinancingType.TrustAndAcademies, ChartFormat format = ChartFormat.Charts)
        {
            ChartGroupType chartGroup;
            switch (tab)
            {
                case RevenueGroupType.Expenditure:
                    chartGroup = ChartGroupType.TotalExpenditure;
                    break;
                case RevenueGroupType.Income:
                    chartGroup = ChartGroupType.TotalIncome;
                    break;
                case RevenueGroupType.Balance:
                    chartGroup = ChartGroupType.InYearBalance;
                    break;
                default:
                    chartGroup = ChartGroupType.All;
                    break;
            }
         
            var academies = _financialDataService.GetAcademiesByCompanyNumber(LatestMATTerm(), companyNo);

            if(academies.Count == 0)
            {
                return RedirectToActionPermanent("SuggestTrust", "TrustSearch", new RouteValueDictionary { { "trustNameId", companyNo } });
            }
 
            var trustVM = await BuildTrustVMAsync(companyNo, academies.First().TrustName, academies, tab, chartGroup, financing);

            List<string> terms = _financialDataService.GetActiveTermsForMatCentral();
            var latestTerm = terms.First();

            UnitType unitType;
            switch (tab)
            {
                case RevenueGroupType.Workforce:
                    unitType = UnitType.AbsoluteCount;
                    break;
                case RevenueGroupType.Balance:
                    unitType = unit == UnitType.AbsoluteMoney || unit == UnitType.PerPupil || unit == UnitType.PerTeacher ? unit : UnitType.AbsoluteMoney;
                    break;
                default:
                    unitType = unit;
                    break;
            }

            _fcService.PopulateHistoricalChartsWithSchoolData(trustVM.HistoricalCharts, trustVM.HistoricalFinancialDataModels, latestTerm, tab, unitType, EstablishmentType.Academies);

            ViewBag.Tab = tab;
            ViewBag.ChartGroup = chartGroup;
            ViewBag.UnitType = unitType;
            ViewBag.Financing = financing;
            ViewBag.ChartFormat = format;
            ViewBag.EstablishmentType = EstablishmentType.MAT;

            return View(trustVM);
        }

        public async Task<PartialViewResult> GetCharts(int companyNo, string name, RevenueGroupType revGroup, ChartGroupType chartGroup, UnitType unit, MatFinancingType financing = MatFinancingType.TrustAndAcademies, ChartFormat format = ChartFormat.Charts)
        {
            var dataResponse = _financialDataService.GetAcademiesByCompanyNumber(LatestMATTerm(), companyNo);

            var trustVM = await BuildTrustVMAsync(companyNo, name, dataResponse, revGroup, chartGroup, financing);

            _fcService.PopulateHistoricalChartsWithSchoolData(trustVM.HistoricalCharts, trustVM.HistoricalFinancialDataModels, LatestMATTerm(), revGroup, unit, EstablishmentType.MAT);

            ViewBag.ChartFormat = format;
            ViewBag.EstablishmentType = EstablishmentType.MAT;
            ViewBag.UnitType = unit;
            ViewBag.Financing = financing;

            return PartialView("Partials/Chart", trustVM);
        }

        public async Task<ActionResult> Download(int companyNo, string name)
        {
            var latestYear = _financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.MAT);
            var term = FormatHelpers.FinancialTermFormatAcademies(latestYear);

            var response = _financialDataService.GetAcademiesByCompanyNumber(term, companyNo);

            var trustVM = await BuildTrustVMAsync(companyNo, name, response, RevenueGroupType.AllExcludingSchoolPerf, ChartGroupType.All, MatFinancingType.TrustOnly);

            var termsList = _financialDataService.GetActiveTermsForMatCentral();
            _fcService.PopulateHistoricalChartsWithSchoolData(trustVM.HistoricalCharts, trustVM.HistoricalFinancialDataModels, termsList.First(), RevenueGroupType.AllExcludingSchoolPerf, UnitType.AbsoluteMoney, EstablishmentType.MAT);
            
            string csv = _csvBuilder.BuildCSVContentHistorically(trustVM, latestYear);

            return File(Encoding.UTF8.GetBytes(csv),
                         "text/plain",
                         $"HistoricalData-{name}.csv");
        }

        private async Task<TrustViewModel> BuildTrustVMAsync(int companyNo, string name, List<AcademiesContextualDataObject> academiesList, RevenueGroupType tab, ChartGroupType chartGroup, MatFinancingType matFinancing)
        {
            var comparisonListVM = _benchmarkBasketCookieManager.ExtractSchoolComparisonListFromCookie();
            var trustVM = new TrustViewModel(companyNo, name, academiesList, comparisonListVM);
            
            trustVM.HistoricalCharts = _historicalChartBuilder.Build(tab, chartGroup, trustVM.EstablishmentType);
            trustVM.ChartGroups = _historicalChartBuilder.Build(tab, trustVM.EstablishmentType).DistinctBy(c => c.ChartGroup).ToList();
            trustVM.LatestTerm = LatestMATTerm();

            trustVM.HistoricalFinancialDataModels = await this.GetFinancialDataHistoricallyAsync(trustVM.CompanyNo, matFinancing);

            if (trustVM.HistoricalFinancialDataModels.Count > 0)
            {
                trustVM.TotalRevenueIncome = trustVM.HistoricalFinancialDataModels.Last().TotalIncome;
                trustVM.TotalRevenueExpenditure = trustVM.HistoricalFinancialDataModels.Last().TotalExpenditure;
                trustVM.InYearBalance = trustVM.HistoricalFinancialDataModels.Last().InYearBalance;
            }
            return trustVM;
        }

        private async Task<List<FinancialDataModel>> GetFinancialDataHistoricallyAsync(int companyNo, MatFinancingType matFinancing)
        {
            var models = new List<FinancialDataModel>();
            var latestYear = _financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.MAT);

            var taskList = new List<Task<IEnumerable<SchoolTrustFinancialDataObject>>>();
            for (int i = ChartHistory.YEARS_OF_HISTORY - 1; i >= 0; i--)
            {
                var term = FormatHelpers.FinancialTermFormatAcademies(latestYear - i);
                var task = _financialDataService.GetTrustFinancialDataObjectAsync(companyNo, term, matFinancing);
                taskList.Add(task);
            }

            for (int i = ChartHistory.YEARS_OF_HISTORY - 1; i >= 0; i--)
            {
                var term = FormatHelpers.FinancialTermFormatAcademies(latestYear - i);
                var taskResult = await taskList[ChartHistory.YEARS_OF_HISTORY - 1 - i];
                var resultObject = taskResult?.FirstOrDefault();

                if (resultObject != null && resultObject.DidNotSubmit)
                {
                    var emptyObj = new SchoolTrustFinancialDataObject();
                    emptyObj.DidNotSubmit = true;
                    resultObject = emptyObj;
                }

                models.Add(new FinancialDataModel(companyNo.ToString(), term, resultObject, EstablishmentType.MAT));
            }

            return models;
        }

        private string LatestMATTerm()
        {
            var latestYear = _financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.MAT);
            return FormatHelpers.FinancialTermFormatAcademies(latestYear);
        }
    }
}