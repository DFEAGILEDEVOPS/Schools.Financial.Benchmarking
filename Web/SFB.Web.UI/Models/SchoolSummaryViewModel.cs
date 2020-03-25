using System;
using SFB.Web.ApplicationCore.Models;

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

        public SchoolSummaryViewModel(SchoolSearchResult model)
        {
            var location = model.Location;
            if (location != null)
            {
                Latitude = location.Latitude;
                Longitude = location.Longitude;
            }
            Name = model.EstablishmentName;
            Id = model.URN;
            Address = String.Format("{0}, {1}, {2}", model.Street, model.Town, model.Postcode);
            EducationPhases = model.PhaseOfEducation;
            OverallPhase = model.OverallPhase;
            NFType = model.TypeOfEstablishment;            
            SponsorName = model.Trusts;
            CompanyNumber = model.CompanyNumber;
        }
    }
}