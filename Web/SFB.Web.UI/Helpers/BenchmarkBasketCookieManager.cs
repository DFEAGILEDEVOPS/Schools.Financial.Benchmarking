using Newtonsoft.Json;
using SFB.Web.Common;
using SFB.Web.Domain.Helpers.Constants;
using SFB.Web.Domain.Services.DataAccess;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFB.Web.UI.Helpers
{
    public class BenchmarkBasketCookieManager : IBenchmarkBasketCookieManager
    {
        private readonly IFinancialDataService _financialDataService;

        public BenchmarkBasketCookieManager(IFinancialDataService financialDataService)
        {
            _financialDataService = financialDataService;
        }

        public SchoolComparisonListModel ExtractSchoolComparisonListFromCookie()
        {
            SchoolComparisonListModel comparisonList = new SchoolComparisonListModel();
            var cookie = HttpContext.Current.Request.Cookies[CookieNames.COMPARISON_LIST];
            if (cookie != null)
            {
                comparisonList = JsonConvert.DeserializeObject<SchoolComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
            }
            return comparisonList;
        }

        public void UpdateSchoolComparisonListCookie(CookieActions withAction, BenchmarkSchoolModel benchmarkSchool)
        {
            HttpCookie cookie = null;

            switch (withAction)
            {
                case CookieActions.Add:
                    cookie = HttpContext.Current.Request.Cookies[CookieNames.COMPARISON_LIST];
                    if (cookie == null)
                    {
                        cookie = new HttpCookie(CookieNames.COMPARISON_LIST);
                        var listCookie = new SchoolComparisonListModel();
                        listCookie.BenchmarkSchools = new List<BenchmarkSchoolModel>() { benchmarkSchool };
                        cookie.Value = JsonConvert.SerializeObject(listCookie, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
                    }
                    else
                    {
                        var listCookie = JsonConvert.DeserializeObject<SchoolComparisonListModel>(cookie.Value, new JsonSerializerSettings() {StringEscapeHandling = StringEscapeHandling.EscapeHtml });
                        if ((listCookie.BenchmarkSchools.Count < ComparisonListLimit.LIMIT || listCookie.HomeSchoolUrn == benchmarkSchool.Urn) && !listCookie.BenchmarkSchools.Any(s => s.Urn == benchmarkSchool.Urn))
                        {
                            listCookie.BenchmarkSchools.Add(benchmarkSchool);
                        }
                        cookie.Value = JsonConvert.SerializeObject(listCookie, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
                    }
                    break;
                case CookieActions.Remove:
                    cookie = HttpContext.Current.Request.Cookies[CookieNames.COMPARISON_LIST];
                    if (cookie != null)
                    {
                        var listCookie = JsonConvert.DeserializeObject<SchoolComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
                        listCookie.BenchmarkSchools.Remove(benchmarkSchool);
                        if (listCookie.HomeSchoolUrn == benchmarkSchool.Urn)
                        {
                            listCookie.HomeSchoolUrn = null;
                            listCookie.HomeSchoolName = null;
                            listCookie.HomeSchoolType = null;
                            listCookie.HomeSchoolFinancialType = null;
                        }
                        cookie.Value = JsonConvert.SerializeObject(listCookie, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
                    }
                    break;
                case CookieActions.SetDefault:
                    cookie = HttpContext.Current.Request.Cookies[CookieNames.COMPARISON_LIST];
                    if (cookie == null)
                    {
                        cookie = new HttpCookie(CookieNames.COMPARISON_LIST);
                        var listCookie = new SchoolComparisonListModel();
                        listCookie.HomeSchoolUrn = benchmarkSchool.Urn;
                        listCookie.HomeSchoolName = benchmarkSchool.Name;
                        listCookie.HomeSchoolType = benchmarkSchool.Type;
                        listCookie.HomeSchoolFinancialType = benchmarkSchool.EstabType;
                        listCookie.BenchmarkSchools = new List<BenchmarkSchoolModel>() { benchmarkSchool };
                        cookie.Value = JsonConvert.SerializeObject(listCookie, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
                    }
                    else
                    {
                        var listCookie = JsonConvert.DeserializeObject<SchoolComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
                        listCookie.HomeSchoolUrn = benchmarkSchool.Urn;
                        listCookie.HomeSchoolName = benchmarkSchool.Name;
                        listCookie.HomeSchoolType = benchmarkSchool.Type;
                        listCookie.HomeSchoolFinancialType = benchmarkSchool.EstabType;
                        if (listCookie.BenchmarkSchools.Count < ComparisonListLimit.LIMIT && listCookie.BenchmarkSchools.All(s => s.Urn != benchmarkSchool.Urn))
                        {
                            listCookie.BenchmarkSchools.Add(benchmarkSchool);
                        }
                        cookie.Value = JsonConvert.SerializeObject(listCookie, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
                    }
                    break;
                case CookieActions.UnsetDefault:
                    cookie = HttpContext.Current.Request.Cookies[CookieNames.COMPARISON_LIST];
                    if (cookie != null)
                    {
                        var listCookie = JsonConvert.DeserializeObject<SchoolComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
                        listCookie.HomeSchoolUrn = null;
                        listCookie.HomeSchoolName = null;
                        listCookie.HomeSchoolType = null;
                        listCookie.HomeSchoolFinancialType = null;
                        cookie.Value = JsonConvert.SerializeObject(listCookie, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
                    }
                    break;
                case CookieActions.RemoveAll:
                    cookie = HttpContext.Current.Request.Cookies[CookieNames.COMPARISON_LIST];
                    if (cookie != null)
                    {
                        var listCookie = JsonConvert.DeserializeObject<SchoolComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
                        listCookie.BenchmarkSchools = new List<BenchmarkSchoolModel>();
                        cookie.Value = JsonConvert.SerializeObject(listCookie, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
                    }
                    break;
            }

            if (cookie != null)
            {
                cookie.Expires = DateTime.MaxValue;
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        public TrustComparisonListModel ExtractTrustComparisonListFromCookie()
        {
            TrustComparisonListModel comparisonList = null;
            var cookie = HttpContext.Current.Request.Cookies[CookieNames.COMPARISON_LIST_MAT];
            if (cookie != null)
            {
                comparisonList = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
            }
            return comparisonList;
        }

        public TrustComparisonListModel UpdateTrustComparisonListCookie(CookieActions withAction, int? companyNo = null, string matName = null)
        {
            TrustComparisonListModel comparisonList = null;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[CookieNames.COMPARISON_LIST_MAT];
            switch (withAction)
            {
                case CookieActions.SetDefault:
                    if (cookie == null)
                    {
                        cookie = new HttpCookie(CookieNames.COMPARISON_LIST_MAT);
                        comparisonList = new TrustComparisonListModel(companyNo.GetValueOrDefault(), matName)
                        {
                            Trusts = new List<BenchmarkTrustModel> { new BenchmarkTrustModel(companyNo.GetValueOrDefault(), matName) }
                        };
                    }
                    else
                    {
                        comparisonList = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
                        RetrieveCompanyNumbers(comparisonList);
                        comparisonList.DefaultTrustCompanyNo = companyNo.GetValueOrDefault();
                        comparisonList.DefaultTrustName = matName;
                        if (comparisonList.Trusts.All(s => s.CompanyNo != companyNo))
                        {
                            comparisonList.Trusts.Add(new BenchmarkTrustModel(companyNo.GetValueOrDefault(), matName));
                        }
                    }
                    break;

                case CookieActions.Add:
                    if (cookie == null)
                    {
                        cookie = new HttpCookie(CookieNames.COMPARISON_LIST_MAT);
                        comparisonList = new TrustComparisonListModel(companyNo.GetValueOrDefault(), matName)
                        {
                            Trusts = new List<BenchmarkTrustModel> { new BenchmarkTrustModel(companyNo.GetValueOrDefault(), matName) }
                        };
                    }
                    else
                    {
                        comparisonList = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
                        if (comparisonList.DefaultTrustCompanyNo == companyNo || comparisonList.Trusts.Any(s => s.CompanyNo == companyNo))
                        {
                            throw new ApplicationException(ErrorMessages.DuplicateTrust);                            
                        }
                        else
                        {
                            comparisonList.Trusts.Add(new BenchmarkTrustModel(companyNo.GetValueOrDefault(), matName));
                        }
                    }
                    break;
                case CookieActions.Remove:
                    comparisonList = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
                    comparisonList.Trusts.Remove(new BenchmarkTrustModel(companyNo.GetValueOrDefault()));
                    break;
                case CookieActions.RemoveAll:
                    comparisonList = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
                    comparisonList.Trusts.Clear();
                    break;
                case CookieActions.AddDefaultToList:
                    comparisonList = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
                    if (comparisonList.Trusts.All(s => comparisonList.DefaultTrustCompanyNo != companyNo))
                    {
                        comparisonList.Trusts.Add(new BenchmarkTrustModel(comparisonList.DefaultTrustCompanyNo, comparisonList.DefaultTrustName));
                    }
                    break;
            }

            cookie.Value = JsonConvert.SerializeObject(comparisonList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
            cookie.Expires = DateTime.MaxValue;
            HttpContext.Current.Response.Cookies.Add(cookie);

            return comparisonList;
        }


        /// <summary>
        /// TODO: Temporary method to update cookies seamlessly, can be removed after release
        ///  Retrieving CompanyNumbers from DB for old cookies which don't have them
        /// </summary>
        /// <param name="comparisonList"></param>
        public void RetrieveCompanyNumbers(TrustComparisonListModel comparisonList)
        {
            var latestYear = _financialDataService.GetLatestDataYearPerEstabType(EstablishmentType.MAT);
            var term = FormatHelpers.FinancialTermFormatAcademies(latestYear-1);

            if(comparisonList.DefaultTrustCompanyNo == 0)
            {
                var financialDataObject = _financialDataService.GetTrustFinancialDataObjectByMatName(comparisonList.DefaultTrustName, term, MatFinancingType.TrustOnly);
                comparisonList.DefaultTrustCompanyNo = financialDataObject.CompanyNumber.GetValueOrDefault();
            }

            foreach (var trust in comparisonList.Trusts)
            {
                if (trust.CompanyNo == 0)
                {
                    var financialDataObject = _financialDataService.GetTrustFinancialDataObjectByMatName(trust.MatName, term, MatFinancingType.TrustOnly);
                    if (financialDataObject == null)
                    {
                        financialDataObject = _financialDataService.GetTrustFinancialDataObjectByMatName(trust.MatName.Replace("The ", ""), term, MatFinancingType.TrustOnly);
                    }
                    trust.CompanyNo = financialDataObject.CompanyNumber.GetValueOrDefault();
                }
            }
            HttpCookie cookie = HttpContext.Current.Request.Cookies[CookieNames.COMPARISON_LIST_MAT];
            cookie.Value = JsonConvert.SerializeObject(comparisonList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
            cookie.Expires = DateTime.MaxValue;
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        private string FormatTerm(string term, EstablishmentType estabType)
        {
            return estabType == EstablishmentType.Academies || estabType == EstablishmentType.MAT ? term : term.Replace('/', '-');
        }
    }
}
