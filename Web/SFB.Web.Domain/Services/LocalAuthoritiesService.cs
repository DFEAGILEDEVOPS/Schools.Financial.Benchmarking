using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace SFB.Web.Domain.Services
{
    public class LocalAuthoritiesService : ILocalAuthoritiesService
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
    }
}