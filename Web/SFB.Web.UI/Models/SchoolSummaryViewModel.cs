using System;
using SFB.Web.ApplicationCore.Entities;
using SFB.Web.ApplicationCore.Models;

namespace SFB.Web.UI.Models
{
    public class SchoolSummaryViewModel
    {
        private EdubaseDataObject result;
        private SchoolViewModel school;

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
            Address = model.FullAddress;
            EducationPhases = model.PhaseOfEducation;
            OverallPhase = model.OverallPhase;
            NFType = model.TypeOfEstablishment;            
            SponsorName = model.Trusts;
            CompanyNumber = model.CompanyNumber;
        }

        public SchoolSummaryViewModel(SchoolViewModel model)
        {
            var location = model.GetLocation();
            if (location != null)
            {
                Latitude = double.Parse(location.coordinates[1]);
                Longitude = double.Parse(location.coordinates[0]);
            }
            Name = model.Name;
            Id = model.Id.ToString();
            Address = model.Address;
            OverallPhase = model.OverallPhase;
            NFType = model.Type;
        }
    }
}