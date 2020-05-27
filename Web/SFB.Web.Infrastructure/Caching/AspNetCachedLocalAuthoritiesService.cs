using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using Newtonsoft.Json;

namespace SFB.Web.ApplicationCore.Services
{
    public class AspNetCachedLocalAuthoritiesService : ILocalAuthoritiesService
    {
        public dynamic GetLocalAuthorities()
        {
            var list = (dynamic)HttpContext.Current.Cache.Get("SFBLARegionList");

            if (list == null)
            {
                list = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/App_data/la.json"));

                HttpContext.Current.Cache.Insert("SFBLARegionList", list, null, DateTime.Now.AddDays(1),
                    Cache.NoSlidingExpiration);
            }

            return list;
        }

        public string GetLaName(string laCode)
        {
            var localAuthorities = (List<dynamic>)JsonConvert.DeserializeObject<List<dynamic>>(GetLocalAuthorities());
            var localAuthourity = localAuthorities.FirstOrDefault(la => la.id == laCode);
            return localAuthourity == null ? string.Empty : localAuthourity.LANAME;
        }
    }
}