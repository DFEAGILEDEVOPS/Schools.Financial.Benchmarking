using Newtonsoft.Json;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Services;
using StackExchange.Redis;
using System;
using System.Configuration;

namespace SFB.Web.Infrastructure.Caching
{
    public class RedisCachedBicComparisonResultCachingService : IBicComparisonResultCachingService
    {

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

        public ComparisonResult GetBicComparisonResultByUrn(int urn)
        {
            var cache = Connection.GetDatabase();

            var serializedList = cache.StringGet("BicComparisonResult-"+urn);

            ComparisonResult deserializedList = null;

            if (!serializedList.IsNull)
            {
                deserializedList = JsonConvert.DeserializeObject<ComparisonResult>(serializedList);
            }

            return deserializedList;
        }

        public void StoreBicComparisonResultByUrn(int urn, ComparisonResult comparisonResult)
        {
            var cache = Connection.GetDatabase();

            cache.StringSet("BicComparisonResult-" + urn, JsonConvert.SerializeObject(comparisonResult));
        }
    }
}
