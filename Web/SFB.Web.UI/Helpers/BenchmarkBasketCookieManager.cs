using Newtonsoft.Json;
using SFB.Web.ApplicationCore.Helpers.Constants;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SFB.Web.ApplicationCore.Helpers.Enums;
using System.Globalization;

namespace SFB.Web.UI.Helpers
{
    public class BenchmarkBasketCookieManager : IBenchmarkBasketCookieManager
    {
        public BenchmarkBasketCookieManager()
        {
        }

        public SchoolComparisonListModel ExtractSchoolComparisonListFromCookie()
        {
            var comparisonList = new SchoolComparisonListModel();
            var cookie = HttpContext.Current.Request.Cookies[CookieNames.COMPARISON_LIST];
            if (cookie != null)
            {
                comparisonList = JsonConvert.DeserializeObject<SchoolComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Culture = new CultureInfo("en-GB", true) });
            }
            return comparisonList;
        }

        public TrustComparisonListModel ExtractTrustComparisonListFromCookie()
        {
            var comparisonList = new TrustComparisonListModel(); ;
            var cookie = HttpContext.Current.Request.Cookies[CookieNames.COMPARISON_LIST_MAT];
            if (cookie != null)
            {
                comparisonList = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Culture = new CultureInfo("en-GB", true) });
            }
            return comparisonList;
        }

        public SchoolComparisonListModel ExtractManualComparisonListFromCookie()
        {
            SchoolComparisonListModel comparisonList = new SchoolComparisonListModel();
            var cookie = HttpContext.Current.Request.Cookies[CookieNames.COMPARISON_LIST_MANUAL];
            if (cookie != null)
            {
                comparisonList = JsonConvert.DeserializeObject<SchoolComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Culture = new CultureInfo("en-GB", true) });
            }
            return comparisonList;
        }

        public void UpdateSchoolComparisonListCookie(CookieActions withAction, BenchmarkSchoolModel benchmarkSchool)
        {
            HttpCookie cookie = null;

            switch (withAction)
            {
                case CookieActions.Add:
                    cookie = AddSchoolToCookie(benchmarkSchool, CookieNames.COMPARISON_LIST);
                    break;
                case CookieActions.Remove:
                    cookie = RemoveSchoolFromCookie(benchmarkSchool, CookieNames.COMPARISON_LIST);
                    break;
                case CookieActions.SetDefault:
                    cookie = SetDefaultSchoolInCookie(benchmarkSchool, CookieNames.COMPARISON_LIST);
                    break;
                case CookieActions.UnsetDefault:
                    cookie = UnsetDefaultSchoolInCookie(CookieNames.COMPARISON_LIST);
                    break;
                case CookieActions.RemoveAll:
                    cookie = RemoveAllSchoolsFromCookie(CookieNames.COMPARISON_LIST);
                    break;
                case CookieActions.AddDefaultToList:
                    cookie = AddDefaultSchoolToListInCookie(CookieNames.COMPARISON_LIST);
                    break;
            }

            if (cookie != null)
            {
                cookie.Expires = DateTime.MaxValue;
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        public void UpdateManualComparisonListCookie(CookieActions withAction, BenchmarkSchoolModel benchmarkSchool)
        {
            HttpCookie cookie = null;

            switch (withAction)
            {
                case CookieActions.Add:
                    cookie = AddSchoolToCookie(benchmarkSchool, CookieNames.COMPARISON_LIST_MANUAL);
                    break;
                case CookieActions.Remove:
                    cookie = RemoveSchoolFromCookie(benchmarkSchool, CookieNames.COMPARISON_LIST_MANUAL);
                    break;
                case CookieActions.SetDefault:
                    cookie = SetDefaultSchoolInCookie(benchmarkSchool, CookieNames.COMPARISON_LIST_MANUAL);
                    break;
                case CookieActions.UnsetDefault:
                    cookie = UnsetDefaultSchoolInCookie(CookieNames.COMPARISON_LIST_MANUAL);
                    break;
                case CookieActions.RemoveAll:
                    cookie = RemoveAllSchoolsFromCookie(CookieNames.COMPARISON_LIST_MANUAL);
                    break;
                case CookieActions.AddDefaultToList:
                    cookie = AddDefaultSchoolToListInCookie(CookieNames.COMPARISON_LIST_MANUAL);
                    break;
            }

            if (cookie != null)
            {
                cookie.HttpOnly = false;
                cookie.Secure = HttpContext.Current.Request.IsSecureConnection;
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
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
                        comparisonList = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Culture = new CultureInfo("en-GB", true) });
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
                        comparisonList = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Culture = new CultureInfo("en-GB", true) });
                        if (comparisonList.Trusts.Any(s => s.CompanyNo == companyNo))
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
                    comparisonList = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Culture = new CultureInfo("en-GB", true) });
                    comparisonList.Trusts.Remove(new BenchmarkTrustModel(companyNo.GetValueOrDefault()));
                    break;
                case CookieActions.RemoveAll:
                    if (cookie != null)
                    {
                        comparisonList = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Culture = new CultureInfo("en-GB", true) });
                        comparisonList.Trusts.Clear();
                    }
                    break;
                case CookieActions.AddDefaultToList:
                    comparisonList = JsonConvert.DeserializeObject<TrustComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Culture = new CultureInfo("en-GB", true) });
                    if (comparisonList.Trusts.All(s => comparisonList.DefaultTrustCompanyNo != companyNo))
                    {
                        comparisonList.Trusts.Add(new BenchmarkTrustModel(comparisonList.DefaultTrustCompanyNo, comparisonList.DefaultTrustName));
                    }
                    break;
            }

            if (cookie != null)
            {
                cookie.Value = JsonConvert.SerializeObject(comparisonList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Culture = new CultureInfo("en-GB", true) });
                cookie.Expires = DateTime.MaxValue;
                cookie.HttpOnly = false;
                cookie.Secure = HttpContext.Current.Request.IsSecureConnection;
                HttpContext.Current.Response.Cookies.Add(cookie);
                return comparisonList;
            }

            return null;            
        }

        private string FormatTerm(string term, EstablishmentType estabType)
        {
            return estabType == EstablishmentType.Academies || estabType == EstablishmentType.MAT ? term : term.Replace('/', '-');
        }

        private HttpCookie AddDefaultSchoolToListInCookie(string cookieName)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null)
            {
                var comparisonList = JsonConvert.DeserializeObject<SchoolComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Culture = new CultureInfo("en-GB", true) });
                if (comparisonList.BenchmarkSchools.All(s => s.Urn != comparisonList.HomeSchoolUrn))
                {
                    AddSchoolToCookie(new BenchmarkSchoolModel() {
                        Urn = comparisonList.HomeSchoolUrn,
                        Name = comparisonList.HomeSchoolName,
                        Type = comparisonList.HomeSchoolType,
                        EstabType = comparisonList.HomeSchoolFinancialType
                    }, cookieName);
                }
            }

            return cookie;
        }

        private HttpCookie RemoveAllSchoolsFromCookie(string cookieName)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null)
            {
                var listCookie = JsonConvert.DeserializeObject<SchoolComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Culture = new CultureInfo("en-GB", true) });
                listCookie.BenchmarkSchools = new List<BenchmarkSchoolModel>();
                cookie.Value = JsonConvert.SerializeObject(listCookie, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Culture = new CultureInfo("en-GB", true) });
            }

            return cookie;
        }

        private HttpCookie UnsetDefaultSchoolInCookie(string cookieName)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null)
            {
                var listCookie = JsonConvert.DeserializeObject<SchoolComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Culture = new CultureInfo("en-GB", true) });
                listCookie.HomeSchoolUrn = null;
                listCookie.HomeSchoolName = null;
                listCookie.HomeSchoolType = null;
                listCookie.HomeSchoolFinancialType = null;
                cookie.Value = JsonConvert.SerializeObject(listCookie, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Culture = new CultureInfo("en-GB", true) });
            }

            return cookie;
        }

        private HttpCookie SetDefaultSchoolInCookie(BenchmarkSchoolModel benchmarkSchool, string cookieName)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie == null)
            {
                cookie = new HttpCookie(cookieName);
                var listCookie = new SchoolComparisonListModel();
                listCookie.HomeSchoolUrn = benchmarkSchool.Urn;
                listCookie.HomeSchoolName = benchmarkSchool.Name;
                listCookie.HomeSchoolType = benchmarkSchool.Type;
                listCookie.HomeSchoolFinancialType = benchmarkSchool.EstabType;
                listCookie.BenchmarkSchools = new List<BenchmarkSchoolModel>() { benchmarkSchool };
                cookie.Value = JsonConvert.SerializeObject(listCookie, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Culture = new CultureInfo("en-GB", true) });
            }
            else
            {
                var listCookie = JsonConvert.DeserializeObject<SchoolComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Culture = new CultureInfo("en-GB", true) });
                listCookie.HomeSchoolUrn = benchmarkSchool.Urn;
                listCookie.HomeSchoolName = benchmarkSchool.Name;
                listCookie.HomeSchoolType = benchmarkSchool.Type;
                listCookie.HomeSchoolFinancialType = benchmarkSchool.EstabType;
                if (benchmarkSchool.Urn != null && listCookie.BenchmarkSchools.Count < ComparisonListLimit.LIMIT && listCookie.BenchmarkSchools.All(s => s.Urn != benchmarkSchool.Urn))
                {
                    listCookie.BenchmarkSchools.Add(benchmarkSchool);
                }
                cookie.Value = JsonConvert.SerializeObject(listCookie, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Culture = new CultureInfo("en-GB", true)  });
            }

            cookie.HttpOnly = false;
            cookie.Secure = HttpContext.Current.Request.IsSecureConnection;
            return cookie;
        }

        private HttpCookie RemoveSchoolFromCookie(BenchmarkSchoolModel benchmarkSchool, string cookieName)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null)
            {
                var listCookie = JsonConvert.DeserializeObject<SchoolComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Culture = new CultureInfo("en-GB", true) });
                listCookie.BenchmarkSchools.Remove(benchmarkSchool);
                if (listCookie.HomeSchoolUrn == benchmarkSchool.Urn)
                {
                    listCookie.HomeSchoolUrn = null;
                    listCookie.HomeSchoolName = null;
                    listCookie.HomeSchoolType = null;
                    listCookie.HomeSchoolFinancialType = null;
                }
                cookie.Value = JsonConvert.SerializeObject(listCookie, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Culture = new CultureInfo("en-GB", true) });
            }

            return cookie;
        }

        private HttpCookie AddSchoolToCookie(BenchmarkSchoolModel benchmarkSchool, string cookieName)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie == null)
            {
                cookie = new HttpCookie(cookieName);
                var listCookie = new SchoolComparisonListModel();
                listCookie.BenchmarkSchools = new List<BenchmarkSchoolModel>() { benchmarkSchool };
                cookie.Value = JsonConvert.SerializeObject(listCookie, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Culture = new CultureInfo("en-GB", true) });
            }
            else
            {
                var listCookie = JsonConvert.DeserializeObject<SchoolComparisonListModel>(cookie.Value, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Culture = new CultureInfo("en-GB", true) });
                if (listCookie.BenchmarkSchools.Any(s => s.Id == benchmarkSchool.Id))
                {
                    throw new ApplicationException(ErrorMessages.DuplicateSchool);
                }
                if (listCookie.BenchmarkSchools.Count < ComparisonListLimit.LIMIT || listCookie.HomeSchoolUrn == benchmarkSchool.Urn)
                {
                    if (listCookie.BenchmarkSchools.Any(s => s.Name == benchmarkSchool.Name))
                    {
                        benchmarkSchool.Name += " ";
                    }

                    listCookie.BenchmarkSchools.Add(benchmarkSchool);

                }
                cookie.Value = JsonConvert.SerializeObject(listCookie, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, Culture = new CultureInfo("en-GB", true) });
            }

            cookie.HttpOnly = false;
            cookie.Secure = HttpContext.Current.Request.IsSecureConnection;
            return cookie;
        }
    }
}
