using System.Configuration;

namespace SFB.Web.UI.Helpers
{
    public static class FeatureManager
    {        
        public static bool IsEnabled(string feature)
        {
            bool.TryParse(ConfigurationManager.AppSettings[feature], out bool featureEnabled);
            return featureEnabled;
        }

        public static bool IsDisabled(string feature)
        {
            bool.TryParse(ConfigurationManager.AppSettings[feature], out bool featureEnabled);
            return !featureEnabled;
        }
    }

    public struct Features
    {
        //public static string EfficiencyMetric => "Feature-EfficiencyMetric-enabled";
        public static string RevisedSchoolPage => "Feature-RevisedSchoolPage-enabled";
    }
}