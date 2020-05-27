using System.Collections.Generic;

namespace SFB.Web.ApplicationCore.Helpers.Constants
{
    public class Dictionaries
    {
        public static Dictionary<int, string> UrbanRuralDictionary => new Dictionary<int, string>()
        {
            {1, "Hamlet and isolated dwelling" },
            {2, "Rural and village" },
            {3, "Town and fringe" },
            {4, "Urban and city"},
            {5, "Conurbation" }
        };
    }
}
