using Microsoft.Spatial;
using System.Collections.Generic;

namespace SFB.Web.ApplicationCore.Models
{
    public class SearchResultsModel<T>
    {
        public SearchResultsModel(int numberOfResults, Dictionary<string, FacetResultModel[]> facets, IEnumerable<T> results,
            int taken,
            int skipped)
        {
            Facets = facets;
            NumberOfResults = numberOfResults;
            Taken = taken;
            Skipped = skipped;
            Results = results;
        }

        public string ErrorMessage { get; set; }
        public int NumberOfResults { get; set; }
        public IEnumerable<T> Results { get; set; }
        public int Taken { get; private set; }
        public int Skipped { get; private set; }
        public Dictionary<string, FacetResultModel[]> Facets { get; set; }
        public string QueryLat { get; set; }
        public string QueryLong { get; set; }
        public bool Disambiguate { get; set; }
    }

    public class TrustSearchResult
    {
        public string TrustOrCompanyName { get; set; }

        public string CompanyNumber { get; set; }

        public string Trusts { get; set; }
    }

    public class SchoolSearchResult
    {
        public string URN { get; set; }
        public string EstablishmentName { get; set; }
        public string EstablishmentNameUpperCase { get; set; }
        public string OverallPhase { get; set; }
        public string PhaseOfEducation { get; set; }
        public string TypeOfEstablishment { get; set; }
        public string Street { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public string Trusts { get; set; }
        public string LACode { get; set; }
        public string EstablishmentNumber { get; set; }
        public string LAEstab { get; set; }
        public string TelephoneNum { get; set; }
        public string NumberOfPupils { get; set; }
        public string StatutoryLowAge { get; set; }
        public string StatutoryHighAge { get; set; }
        public string HeadFirstName { get; set; }
        public string HeadLastName { get; set; }
        public string OfficialSixthForm { get; set; }
        public string SchoolWebsite { get; set; }
        public string OfstedRating { get; set; }
        public string OfstedLastInsp { get; set; }
        public string FinanceType { get; set; }
        public string OpenDate { get; set; }
        public string CloseDate { get; set; }
        public string EstablishmentStatus { get; set; }
        public string EstablishmentStatusInLatestAcademicYear { get; set; }
        public string ReligiousCharacter { get; set; }
        public string Gender { get; set; }
        public string MATNumber { get; set; }
        public string CompanyNumber { get; set; }
        public string NurseryProvisionName { get; set; }
        public GeographyPoint Location { get; set; }

        public double DistanceInMeters;
    }
}
