using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using SFB.Web.ApplicationCore.Services;
using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace SFB.Web.Api
{
    public class NetCoreCachedActiveCollectionsService : IActiveCollectionsService
    {
        private readonly MemoryCache _memoryCache;
        public NetCoreCachedActiveCollectionsService()
        {
            _memoryCache = MemoryCache.Default;
        }

        public List<JObject> GetActiveCollectionsList()
        {
            return (List<JObject>)_memoryCache.Get("SFBActiveCollectionList");
        }

        public void SetActiveCollectionsList(List<JObject> docs)
        { 
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.SlidingExpiration = TimeSpan.FromMinutes(60);
            _memoryCache.Set("SFBActiveCollectionList", docs, policy);
        }
    }
}
