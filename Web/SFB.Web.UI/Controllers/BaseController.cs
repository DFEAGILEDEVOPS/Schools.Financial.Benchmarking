using Newtonsoft.Json;
using SFB.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using SFB.Web.Common;
using SFB.Web.UI.Helpers.Constants;

namespace SFB.Web.UI.Controllers
{
    public class BaseController : Controller
    {
        protected ComparisonListModel ExtractSchoolComparisonListFromCookie()
        {
            ComparisonListModel comparisonList = new ComparisonListModel();
            var cookie = Request.Cookies[CookieNames.COMPARISON_LIST];
            if (cookie != null)
            {
                comparisonList = JsonConvert.DeserializeObject<ComparisonListModel>(cookie.Value);
            }
            return comparisonList;
        }

        public HttpCookie UpdateSchoolComparisonListCookie(string withAction, BenchmarkSchoolViewModel benchmarkSchool)
        {
            HttpCookie cookie = null;

            switch (withAction)
            {
                case CompareActions.ADD_TO_COMPARISON_LIST:
                    cookie = Request.Cookies[CookieNames.COMPARISON_LIST];
                    if (cookie == null)
                    {
                        cookie = new HttpCookie(CookieNames.COMPARISON_LIST);
                        var listCookie = new ComparisonListModel();
                        listCookie.BenchmarkSchools = new List<BenchmarkSchoolViewModel>() { benchmarkSchool };
                        cookie.Value = JsonConvert.SerializeObject(listCookie);
                    }
                    else
                    {
                        var listCookie = JsonConvert.DeserializeObject<ComparisonListModel>(cookie.Value);
                        if ((listCookie.BenchmarkSchools.Count < ComparisonListLimit.LIMIT || listCookie.HomeSchoolUrn == benchmarkSchool.Urn) && !listCookie.BenchmarkSchools.Any(s => s.Urn == benchmarkSchool.Urn))
                        {
                            listCookie.BenchmarkSchools.Add(benchmarkSchool);
                        }
                        cookie.Value = JsonConvert.SerializeObject(listCookie);
                    }
                    break;
                case CompareActions.REMOVE_FROM_COMPARISON_LIST:
                    cookie = Request.Cookies[CookieNames.COMPARISON_LIST];
                    if (cookie != null)
                    {
                        var listCookie = JsonConvert.DeserializeObject<ComparisonListModel>(cookie.Value);
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
                case CompareActions.MAKE_DEFAULT_BENCHMARK:
                    cookie = Request.Cookies[CookieNames.COMPARISON_LIST];
                    if (cookie == null)
                    {
                        cookie = new HttpCookie(CookieNames.COMPARISON_LIST);
                        var listCookie = new ComparisonListModel();
                        listCookie.HomeSchoolUrn = benchmarkSchool.Urn;
                        listCookie.HomeSchoolName = benchmarkSchool.Name;
                        listCookie.HomeSchoolType = benchmarkSchool.Type;
                        listCookie.HomeSchoolFinancialType = benchmarkSchool.FinancialType;
                        listCookie.BenchmarkSchools = new List<BenchmarkSchoolViewModel>() { benchmarkSchool };
                        cookie.Value = JsonConvert.SerializeObject(listCookie);
                    }
                    else
                    {
                        var listCookie = JsonConvert.DeserializeObject<ComparisonListModel>(cookie.Value);
                        listCookie.HomeSchoolUrn = benchmarkSchool.Urn;
                        listCookie.HomeSchoolName = benchmarkSchool.Name;
                        listCookie.HomeSchoolType = benchmarkSchool.Type;
                        listCookie.HomeSchoolFinancialType = benchmarkSchool.FinancialType;
                        if (listCookie.BenchmarkSchools.Count < ComparisonListLimit.LIMIT && listCookie.BenchmarkSchools.All(s => s.Urn != benchmarkSchool.Urn))
                        {
                            listCookie.BenchmarkSchools.Add(benchmarkSchool);
                        }
                        cookie.Value = JsonConvert.SerializeObject(listCookie);
                    }
                    break;
                case CompareActions.REMOVE_DEFAULT_BENCHMARK:
                    cookie = Request.Cookies[CookieNames.COMPARISON_LIST];
                    if (cookie != null)
                    {
                        var listCookie = JsonConvert.DeserializeObject<ComparisonListModel>(cookie.Value);
                        listCookie.HomeSchoolUrn = null;
                        listCookie.HomeSchoolName = null;
                        listCookie.HomeSchoolType = null;
                        listCookie.HomeSchoolFinancialType = null;
                        cookie.Value = JsonConvert.SerializeObject(listCookie);
                    }
                    break;
                case CompareActions.CLEAR_BENCHMARK_LIST:
                    cookie = Request.Cookies[CookieNames.COMPARISON_LIST];
                    if (cookie != null)
                    {
                        var listCookie = JsonConvert.DeserializeObject<ComparisonListModel>(cookie.Value);
                        listCookie.BenchmarkSchools = new List<BenchmarkSchoolViewModel>();
                        cookie.Value = JsonConvert.SerializeObject(listCookie);
                    }
                    break;
            }

            if (cookie != null)
            {
                cookie.Expires = DateTime.MaxValue;
            }
            return cookie;
        }

        protected TrustComparisonViewModel ExtractTrustComparisonListFromCookie()
        {
            TrustComparisonViewModel comparisonList = null;
            var cookie = Request.Cookies[CookieNames.COMPARISON_LIST_MAT];
            if (cookie != null)
            {
                comparisonList = JsonConvert.DeserializeObject<TrustComparisonViewModel>(cookie.Value);
            }
            return comparisonList;
        }

        protected string FormatTerm(string term, SchoolFinancialType financialType)
        {
            return financialType == SchoolFinancialType.Academies ? term : term.Replace('/', '-');
        }
    }
}