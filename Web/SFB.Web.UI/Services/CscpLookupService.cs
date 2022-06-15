using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace SFB.Web.UI.Services
{ 
    public class CscpLookupService : ICscpLookupService
    {
         private static HttpClient _client;
         
         public CscpLookupService(HttpClient client)
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
       
         public async Task<bool> CscpHasPage(int urn, bool isMat)
         {
             var collection = GetCollection(isMat);
             var key = $"cscp-{collection}-{urn}";
             var value = MemoryCache.Default.Get(key);

             if (value != null)
             {
                 return (bool) value;
             }
             else
             {
                 var baseUrl = $"{_client.BaseAddress.AbsoluteUri}";
                
                 baseUrl = isMat ?
                     $"{baseUrl}multi-academy-trust/{urn}": 
                     $"{baseUrl}school/{urn}";
                
                 var request = new HttpRequestMessage(HttpMethod.Head, $"{baseUrl}");

                 var result = await _client.SendAsync(request);

                 var isOk = result.StatusCode == HttpStatusCode.OK;
                
                 MemoryCache.Default.Set(
                     new CacheItem(key, isOk), 
                     new CacheItemPolicy{AbsoluteExpiration = DateTimeOffset.Now.AddHours(Double.Parse(ConfigurationManager.AppSettings["ExternalServiceCacheHours"]))}
                 );

                 return isOk;
             }
         }

    }
}