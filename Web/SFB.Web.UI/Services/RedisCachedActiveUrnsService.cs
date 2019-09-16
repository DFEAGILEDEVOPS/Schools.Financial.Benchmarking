using Newtonsoft.Json;
using SFB.Web.Domain.Services.DataAccess;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace SFB.Web.UI.Services
{
    /// <summary>
    /// This class should be registered as singleton.    
    /// </summary>
    public class RedisCachedActiveUrnsService : IActiveUrnsService
    {
        private readonly IContextDataService _contextDataService;

        /// <summary>
        /// //Clear the active urns cache at each load of the web application(in prod builds). 
        /// </summary>
        public RedisCachedActiveUrnsService(IContextDataService contextDataService)
        {
            _contextDataService = contextDataService;

            #if !DEBUG
               ClearCachedData();
            #endif
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

        public List<int> GetAllActiveUrns()
        {
            var cache = Connection.GetDatabase();
            
            var serializedList = cache.StringGet("SFBActiveURNList");

            List<int> deserializedList;

            if (serializedList.IsNull)
            {
                deserializedList = _contextDataService.GetAllSchoolUrns();

                cache.StringSet("SFBActiveURNList", JsonConvert.SerializeObject(deserializedList));
            }
            else
            {
                deserializedList = JsonConvert.DeserializeObject<List<int>>(serializedList);
            }
            
            return deserializedList;
        }

        private void ClearCachedData()
        {
            var cache = Connection.GetDatabase();
            cache.KeyDelete("SFBActiveURNList");
        }
    }
}