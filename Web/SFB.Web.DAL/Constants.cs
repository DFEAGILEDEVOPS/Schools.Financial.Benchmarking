namespace SFB.Web.DAL
{
    public struct DataGroups
    {
        public const string Edubase = "Edubase";

        public const string Maintained = "Maintained";//Maintained figures

        public const string Academies = "Academies";//Academy's own figures        
        public const string MATAllocs = "MAT-Allocs";//Academy + its MAT's allocated figures        

        public const string MATCentral = "MAT-Central";//MAT only figures
        public const string MATTotals = "MAT-Totals";//Total of Academy only figures of the MAT
        public const string MATOverview = "MAT-Overview";//MAT + all of its Academies' figures

        public const string EfficiencyMetrics = "EfficiencyMetrics";

        public const string Unidentified = "Unidentified";
        
        //public const string MATDistributed = "MAT-Distributed";//retired
    }
}
