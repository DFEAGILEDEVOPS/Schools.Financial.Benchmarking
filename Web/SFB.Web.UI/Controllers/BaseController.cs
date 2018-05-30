using Newtonsoft.Json;
using SFB.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using SFB.Web.Common;
using SFB.Web.Domain.Helpers.Constants;
using SFB.Web.UI.Helpers.Constants;

namespace SFB.Web.UI.Controllers
{
    public class BaseController : Controller
    {
        protected SchoolComparisonListModel ExtractSchoolComparisonListFromCookie()
        {
            SchoolComparisonListModel comparisonList = new SchoolComparisonListModel();
            var cookie = Request.Cookies[CookieNames.COMPARISON_LIST];
            if (cookie != null)
            {
                comparisonList = JsonConvert.DeserializeObject<SchoolComparisonListModel>(cookie.Value);
            }
            return comparisonList;
        }

        public HttpCookie UpdateSchoolComparisonListCookie(string withAction, BenchmarkSchoolModel benchmarkSchool)
        {
            HttpCookie cookie = null;

            switch (withAction)
            {
                case CompareActions.ADD_TO_COMPARISON_LIST:
                    cookie = Request.Cookies[CookieNames.COMPARISON_LIST];
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
                case CompareActions.REMOVE_FROM_COMPARISON_LIST:
                    cookie = Request.Cookies[CookieNames.COMPARISON_LIST];
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
                case CompareActions.MAKE_DEFAULT_BENCHMARK:
                    cookie = Request.Cookies[CookieNames.COMPARISON_LIST];
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
                case CompareActions.REMOVE_DEFAULT_BENCHMARK:
                    cookie = Request.Cookies[CookieNames.COMPARISON_LIST];
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
                case CompareActions.CLEAR_BENCHMARK_LIST:
                    cookie = Request.Cookies[CookieNames.COMPARISON_LIST];
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
            return cookie;
        }

        protected TrustComparisonListModel ExtractTrustComparisonListFromCookie()
        {
            TrustComparisonListModel comparisonList = null;
            var cookie = Request.Cookies[CookieNames.COMPARISON_LIST_MAT];
            if (cookie != null)
            {
                comparisonList = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value);
            }
            return comparisonList;
        }

        protected string FormatTerm(string term, EstablishmentType estabType)
        {
            return estabType == EstablishmentType.Academies || estabType == EstablishmentType.MAT ? term : term.Replace('/', '-');
        }
    }
}