using SFB.Web.Common;
using System;

namespace SFB.Web.UI.Models
{
    public class SchoolSummaryViewModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public string Address { get; set; }
        public string EducationPhases { get; set; }
        public string OverallPhase { get; set; }
        public string NFType { get; set; }
        public string FinanceType { get; set; }
        public string SponsorName { get; set; }
        public string CompanyNumber { get; set; }
        public bool InsideSearchArea { get; set; }

        public SchoolSummaryViewModel(dynamic model)
        {
            var location = model["Location"];
            if (location != null && location.coordinates != null)
            {
                Latitude = location.coordinates[1];
                Longitude = location.coordinates[0];
            }
            Name = model[EdubaseDBFieldNames.ESTAB_NAME];
            Id = model[EdubaseDBFieldNames.URN];
            Address = String.Format("{0}, {1}, {2}", model[EdubaseDBFieldNames.STREET], model[EdubaseDBFieldNames.TOWN], model[EdubaseDBFieldNames.POSTCODE]);
            EducationPhases = model[EdubaseDBFieldNames.PHASE_OF_EDUCATION];
            OverallPhase = model[EdubaseDBFieldNames.OVERALL_PHASE];
            NFType = model[EdubaseDBFieldNames.TYPE_OF_ESTAB];            
            SponsorName = model[EdubaseDBFieldNames.TRUSTS];
            CompanyNumber = model[EdubaseDBFieldNames.COMPANY_NUMBER];
        }
    }
}