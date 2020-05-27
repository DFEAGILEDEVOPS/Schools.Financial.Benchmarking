using Newtonsoft.Json;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.ApplicationCore.Services.DataAccess;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace SFB.Web.Infrastructure.Caching
{
    /// <summary>
    /// This class should be registered as singleton.    
    /// </summary>
    public class RedisCachedActiveUrnsService : IActiveUrnsService
    {
        private readonly IContextDataService _contextDataService;

        public RedisCachedActiveUrnsService(IContextDataService contextDataService)
        {
            _contextDataService = contextDataService;

            //#if !DEBUG
            //   ClearCachedData();
            //#endif
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

        public async Task<List<int>> GetAllActiveUrnsAsync()
        {
            var cache = Connection.GetDatabase();
            
            var serializedList = cache.StringGet("SFBActiveURNList");

            List<int> deserializedList;

            if (serializedList.IsNull)
            {
                deserializedList = await _contextDataService.GetAllSchoolUrnsAsync();

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