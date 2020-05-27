using SFB.Web.ApplicationCore.Entities;

namespace SFB.Web.ApplicationCore.Models
{
    public class EfficiencyMetricNeighbourModel
    {
        private EfficiencyMetricDataObject EMData { get; set; }
        private EdubaseDataObject ContextData { get; set; }

        public EfficiencyMetricNeighbourModel(int urn, int rank, EdubaseDataObject contextData, EfficiencyMetricDataObject efficiencyMetricData)
        {
            Urn = urn;
            Rank = rank;
            ContextData = contextData;
            EMData = efficiencyMetricData;
        }

        public int Urn { get; set; }

        public int Rank { get; set; }

        public string Name => EMData.Name;

        public int LA => EMData.La;

        public string LocalAuthority => EMData.Laname;

        public decimal Pupils => EMData.Fte;

        public decimal Ever6 => EMData.Ever6pub;

        public decimal SEN => EMData.Senpub;

        public decimal IncomePP => EMData.Incomepp;

        public decimal? ProgressReading => EMData.Readprog_supp;

        public decimal? ProgressWriting => EMData.Writprog_supp;

        public decimal? ProgressMaths => EMData.Matprog_supp;

        public decimal? Progress8 => EMData.Progress8;

        public string Address => $"{ContextData.Street}, {ContextData.Town}, {ContextData.Postcode}";

        public string Telephone => ContextData.TelephoneNum;

        public string HeadTeacher => $"{ContextData.HeadFirstName} {ContextData.HeadLastName}";

        public string SchoolType => ContextData.TypeOfEstablishment;

        public string OfstedRating => ContextData.OfstedRating;

        public string ReligiousCharacter => ContextData.ReligiousCharacter;

        public string OverallPhase => ContextData.OverallPhase;

        public LocationDataObject Location => ContextData.Location;
    }
}
