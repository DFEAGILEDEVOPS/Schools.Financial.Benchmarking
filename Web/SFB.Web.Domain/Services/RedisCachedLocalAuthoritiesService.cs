using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using Newtonsoft.Json;
using System.Configuration;
using StackExchange.Redis;

namespace SFB.Web.Domain.Services
{
    /// <summary>
    /// This class should be registered as singleton.    
    /// </summary>
    public class RedisCachedLocalAuthoritiesService : ILocalAuthoritiesService
    {
        /// <summary>
        /// //Clear the LA cache at each load of the web application. 
        /// </summary>
        public RedisCachedLocalAuthoritiesService()
        {
            ClearCachedData();
        }

        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string cacheConnection = ConfigurationManager.AppSettings["RedisConnectionString"].ToString();
            return ConnectionMultiplexer.Connect(cacheConnection);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        public string GetLaName(string laCode)
        {
            var localAuthorities = (List<dynamic>)JsonConvert.DeserializeObject<List<dynamic>>(GetLocalAuthorities());
            var localAuthourity = localAuthorities.FirstOrDefault(la => la.id == laCode);
            return localAuthourity == null ? string.Empty : localAuthourity.LANAME;
        }

        public dynamic GetLocalAuthorities()
        {
            var cache = Connection.GetDatabase();

            var list = cache.StringGet("SFBLARegionList");

            if (list.IsNull)
            {
                list = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/App_data/la.json"));

                cache.StringSet("SFBLARegionList", list);
            }

            return list;
        }

        private void ClearCachedData()
        {
            var cache = Connection.GetDatabase();
            cache.KeyDelete("SFBLARegionList");
        }
    }
}