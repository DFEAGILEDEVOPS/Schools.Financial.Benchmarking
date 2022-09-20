using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace SFB.Web.UI.Services
{
    public class GiasLookupService : IGiasLookupService
    {
        private static HttpClient _client;
        
        public GiasLookupService(HttpClient client)
        {
            _client = client;
        }
        
        private string _matKeyFragment = "multi-academy-trust";
        private string _schoolKeyFragment = "school";
        private string GetCollection(bool mat)
        {
            var collection = mat ? _matKeyFragment : _schoolKeyFragment;
            return collection;
        }

      public async Task<bool> GiasHasPage(int urn, bool isMat)
        {
            var collection = GetCollection(isMat);
            var key = $"gias-{collection}-{urn}";
            var value = MemoryCache.Default.Get(key);
            
            if (value != null)
            {
                return (bool) value;
            }
            else
            {
                var baseUrl = $"{_client.BaseAddress.AbsoluteUri}";
                
                baseUrl = isMat ?
                    $"{baseUrl}Groups/Group/Details/{urn}": 
                    $"{baseUrl}Establishments/Establishment/Details/{urn}";
                
                var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}");

                var result = await _client.SendAsync(request);

                var isOk = result?.StatusCode == HttpStatusCode.OK;
                
                MemoryCache.Default.Set(
                    new CacheItem(key, isOk), 
                    new CacheItemPolicy{AbsoluteExpiration = DateTimeOffset.Now.AddHours(Double.Parse(ConfigurationManager.AppSettings["ExternalServiceCacheHours"]))}
                );

                return isOk;
            }
        }
    }
}