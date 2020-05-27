using System.Collections.Generic;

namespace SFB.Web.ApplicationCore.Models
{
    public class ResultModel
    {
        public int Count;
        public List<string> Urns;

        public ResultModel(dynamic apiResult)
        {
            Count = apiResult.Matches.Count;
            Urns = new List<string>();
            foreach (var item in apiResult.Matches)
            {
                Urns.Add(item.Id.ToString());
            }               
        }
    }
}

