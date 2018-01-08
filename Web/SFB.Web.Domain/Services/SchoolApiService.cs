using SFB.Web.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using SFB.Web.Domain.ApiWrappers;

namespace SFB.Web.Domain.Services
{
    public class SchoolApiService : ISchoolApiService
    {
        private readonly IApiRequest _dfeApiRequest;

        public SchoolApiService(IApiRequest dfeApiRequest)
        {
            _dfeApiRequest = dfeApiRequest;               
        }

        public dynamic GetLocalAuthorities()
        {
            var list = (dynamic) HttpContext.Current.Cache.Get("SFBLARegionList");

            if (list == null)
            {
                var endpoint = "la_region";
                var actions = new List<string>();
                var apiResponse = _dfeApiRequest.Get(endpoint, actions, new Dictionary<string, string>());

                if (apiResponse.statusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new ApplicationException("SPT API failed to return LA/Region list!");
                }

                list = apiResponse.responseObject;

                HttpContext.Current.Cache.Insert("SFBLARegionList", list, null, DateTime.Now.AddDays(1),
                    Cache.NoSlidingExpiration);
            }

            return list;
        }

    }
}
