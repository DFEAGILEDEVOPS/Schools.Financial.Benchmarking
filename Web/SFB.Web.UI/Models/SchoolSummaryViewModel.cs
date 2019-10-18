using SFB.Web.ApplicationCore.Helpers.Constants;
using SFB.Web.ApplicationCore;
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

        public SchoolSummaryViewModel(dynamic model)
        {
            var location = model["Location"];
            if (location != null && location.coordinates != null)
            {
                Latitude = location.coordinates[1];
                Longitude = location.coordinates[0];
            }
            Name = model[EdubaseDataFieldNames.ESTAB_NAME];
            Id = model[EdubaseDataFieldNames.URN];
            Address = String.Format("{0}, {1}, {2}", model[EdubaseDataFieldNames.STREET], model[EdubaseDataFieldNames.TOWN], model[EdubaseDataFieldNames.POSTCODE]);
            EducationPhases = model[EdubaseDataFieldNames.PHASE_OF_EDUCATION];
            OverallPhase = model[EdubaseDataFieldNames.OVERALL_PHASE];
            NFType = model[EdubaseDataFieldNames.TYPE_OF_ESTAB];            
            SponsorName = model[EdubaseDataFieldNames.TRUSTS];
            CompanyNumber = model[EdubaseDataFieldNames.COMPANY_NUMBER];
        }
    }
}