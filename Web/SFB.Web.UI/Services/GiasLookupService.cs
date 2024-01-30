using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Polly;
using Polly.Wrap;

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

        private static readonly AsyncPolicyWrap RetryPolicy = Policy.TimeoutAsync(1).WrapAsync(
            Policy.Handle<HttpRequestException>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                }));

      public async Task<bool> GiasHasPage(int urn, bool isMat)
        {
            var collection = GetCollection(isMat);
            var key = $"gias-{collection}-{urn}";
            var value = MemoryCache.Default.Get(key);
            
            if (value != null)
            {
                return (bool) value;
            }
           
            try
            {
                var baseUrl = $"{_client.BaseAddress.AbsoluteUri}";

                baseUrl = isMat
                    ? $"{baseUrl}Groups/Group/Details/{urn}"
                    : $"{baseUrl}Establishments/Establishment/Details/{urn}";

                System.Diagnostics.Trace.TraceError(baseUrl);

                var requestMethod = isMat ? HttpMethod.Get : HttpMethod.Head;
                
                var request = new HttpRequestMessage(requestMethod, $"{baseUrl}");

                System.Diagnostics.Trace.TraceError("before executeasync");
                var result = await RetryPolicy.ExecuteAsync(async () => await _client.SendAsync(request));
                System.Diagnostics.Trace.TraceError("after executeasync");


                var isOk = result?.StatusCode == HttpStatusCode.OK;

                MemoryCache.Default.Set(
                    new CacheItem(key, isOk),
                    new CacheItemPolicy
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddHours(
                            Double.Parse(ConfigurationManager.AppSettings["ExternalServiceCacheHours"]))
                    }
                );

                return isOk;
                
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine(ex.CancellationToken.IsCancellationRequested);
                throw;
            }
        }
    }
}