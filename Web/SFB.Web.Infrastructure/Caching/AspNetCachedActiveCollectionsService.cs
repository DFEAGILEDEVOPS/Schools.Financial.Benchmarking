using Newtonsoft.Json.Linq;
using SFB.Web.ApplicationCore.Services;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;

namespace SFB.Web.Infrastructure.Caching
{
    public class AspNetCachedActiveCollectionsService : IActiveCollectionsService
    {
        public List<JObject> GetActiveCollectionsList()
        {
            return (List<JObject>)HttpContext.Current.Cache.Get("SFBActiveCollectionList"); 
        }

        public void SetActiveCollectionsList(List<JObject> docs)
        {
            HttpContext.Current.Cache.Insert("SFBActiveCollectionList", docs, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(60));
        }
    }
}
