using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace SFB.Web.ApplicationCore.Services
{
    public interface IActiveCollectionsService
    {
        List<JObject> GetActiveCollectionsList();
        void SetActiveCollectionsList(List<JObject> docs);
    }
}
