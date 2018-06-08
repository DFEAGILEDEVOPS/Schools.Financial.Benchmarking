using Newtonsoft.Json;
using SFB.Web.Common;
using SFB.Web.Domain.Helpers.Constants;
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
        public BenchmarkBasketCookieManager()
        {
        }

        public SchoolComparisonListModel ExtractSchoolComparisonListFromCookie()
        {
            SchoolComparisonListModel comparisonList = new SchoolComparisonListModel();
            var cookie = HttpContext.Current.Request.Cookies[CookieNames.COMPARISON_LIST];
            if (cookie != null)
            {
                comparisonList = JsonConvert.DeserializeObject<SchoolComparisonListModel>(cookie.Value);
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
                        cookie.Value = JsonConvert.SerializeObject(listCookie);
                    }
                    else
                    {
                        var listCookie = JsonConvert.DeserializeObject<SchoolComparisonListModel>(cookie.Value);
                        if ((listCookie.BenchmarkSchools.Count < ComparisonListLimit.LIMIT || listCookie.HomeSchoolUrn == benchmarkSchool.Urn) && !listCookie.BenchmarkSchools.Any(s => s.Urn == benchmarkSchool.Urn))
                        {
                            listCookie.BenchmarkSchools.Add(benchmarkSchool);
                        }
                        cookie.Value = JsonConvert.SerializeObject(listCookie);
                    }
                    break;
                case CookieActions.Remove:
                    cookie = HttpContext.Current.Request.Cookies[CookieNames.COMPARISON_LIST];
                    if (cookie != null)
                    {
                        var listCookie = JsonConvert.DeserializeObject<SchoolComparisonListModel>(cookie.Value);
                        listCookie.BenchmarkSchools.Remove(benchmarkSchool);
                        if (listCookie.HomeSchoolUrn == benchmarkSchool.Urn)
                        {
                            listCookie.HomeSchoolUrn = null;
                            listCookie.HomeSchoolName = null;
                            listCookie.HomeSchoolType = null;
                            listCookie.HomeSchoolFinancialType = null;
                        }
                        cookie.Value = JsonConvert.SerializeObject(listCookie);
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
                        cookie.Value = JsonConvert.SerializeObject(listCookie);
                    }
                    else
                    {
                        var listCookie = JsonConvert.DeserializeObject<SchoolComparisonListModel>(cookie.Value);
                        listCookie.HomeSchoolUrn = benchmarkSchool.Urn;
                        listCookie.HomeSchoolName = benchmarkSchool.Name;
                        listCookie.HomeSchoolType = benchmarkSchool.Type;
                        listCookie.HomeSchoolFinancialType = benchmarkSchool.EstabType;
                        if (listCookie.BenchmarkSchools.Count < ComparisonListLimit.LIMIT && listCookie.BenchmarkSchools.All(s => s.Urn != benchmarkSchool.Urn))
                        {
                            listCookie.BenchmarkSchools.Add(benchmarkSchool);
                        }
                        cookie.Value = JsonConvert.SerializeObject(listCookie);
                    }
                    break;
                case CookieActions.UnsetDefault:
                    cookie = HttpContext.Current.Request.Cookies[CookieNames.COMPARISON_LIST];
                    if (cookie != null)
                    {
                        var listCookie = JsonConvert.DeserializeObject<SchoolComparisonListModel>(cookie.Value);
                        listCookie.HomeSchoolUrn = null;
                        listCookie.HomeSchoolName = null;
                        listCookie.HomeSchoolType = null;
                        listCookie.HomeSchoolFinancialType = null;
                        cookie.Value = JsonConvert.SerializeObject(listCookie);
                    }
                    break;
                case CookieActions.RemoveAll:
                    cookie = HttpContext.Current.Request.Cookies[CookieNames.COMPARISON_LIST];
                    if (cookie != null)
                    {
                        var listCookie = JsonConvert.DeserializeObject<SchoolComparisonListModel>(cookie.Value);
                        listCookie.BenchmarkSchools = new List<BenchmarkSchoolModel>();
                        cookie.Value = JsonConvert.SerializeObject(listCookie);
                    }
                    break;
            }

            if (cookie != null)
            {
                cookie.Expires = DateTime.MaxValue;
            }

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public TrustComparisonListModel ExtractTrustComparisonListFromCookie()
        {
            TrustComparisonListModel comparisonList = null;
            var cookie = HttpContext.Current.Request.Cookies[CookieNames.COMPARISON_LIST_MAT];
            if (cookie != null)
            {
                comparisonList = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value);
            }
            return comparisonList;
        }

        public TrustComparisonListModel UpdateTrustComparisonListCookie(CookieActions withAction, string matNo = null, string matName = null)
        {
            TrustComparisonListModel comparisonList = null;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[CookieNames.COMPARISON_LIST_MAT];
            switch (withAction)
            {
                case CookieActions.SetDefault:
                    if (cookie == null)
                    {
                        cookie = new HttpCookie(CookieNames.COMPARISON_LIST_MAT);
                        comparisonList = new TrustComparisonListModel(matNo, matName)
                        {
                            Trusts = new List<BenchmarkTrustModel> { new BenchmarkTrustModel(matNo, matName) }
                        };
                    }
                    else
                    {
                        comparisonList = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value);
                        comparisonList.DefaultTrustMatNo = matNo;
                        comparisonList.DefaultTrustName = matName;
                        if (comparisonList.Trusts.All(s => s.MatNo != matNo))
                        {
                            comparisonList.Trusts.Add(new BenchmarkTrustModel(matNo, matName));
                        }
                    }
                    break;

                case CookieActions.Add:
                    if (cookie == null)
                    {
                        cookie = new HttpCookie(CookieNames.COMPARISON_LIST_MAT);
                        comparisonList = new TrustComparisonListModel(matNo, matName)
                        {
                            Trusts = new List<BenchmarkTrustModel> { new BenchmarkTrustModel(matNo, matName) }
                        };
                    }
                    else
                    {
                        comparisonList = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value);
                        if (comparisonList.DefaultTrustMatNo == matNo || comparisonList.Trusts.Any(s => s.MatNo == matNo))
                        {
                            throw new ApplicationException(ErrorMessages.DuplicateTrust);                            
                        }
                        else
                        {
                            comparisonList.Trusts.Add(new BenchmarkTrustModel(matNo, matName));
                        }
                    }
                    break;
                case CookieActions.Remove:
                    comparisonList = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value);
                    comparisonList.Trusts.Remove(new BenchmarkTrustModel(matNo));
                    break;
                case CookieActions.RemoveAll:
                    comparisonList = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value);
                    comparisonList.Trusts.Clear();
                    break;
                case CookieActions.AddDefaultToList:
                    comparisonList = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value);
                    if (comparisonList.Trusts.All(s => comparisonList.DefaultTrustMatNo != matNo))
                    {
                        comparisonList.Trusts.Add(new BenchmarkTrustModel(comparisonList.DefaultTrustMatNo, comparisonList.DefaultTrustName));
                    }
                    break;
            }

            cookie.Value = JsonConvert.SerializeObject(comparisonList);
            cookie.Expires = DateTime.MaxValue;
            HttpContext.Current.Response.Cookies.Add(cookie);

            return comparisonList;
        }

        private string FormatTerm(string term, EstablishmentType estabType)
        {
            return estabType == EstablishmentType.Academies || estabType == EstablishmentType.MAT ? term : term.Replace('/', '-');
        }
    }
}
