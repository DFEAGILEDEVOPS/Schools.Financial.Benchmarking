using RestSharp;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Services;
using System.Collections.Generic;
using System.Web.Configuration;

namespace SFB.Web.UI.Services
{
    public class AzureMapsLocationSearchService : ILocationSearchService
    {
        private const string AzureGeocodingUrl = "https://atlas.microsoft.com";

        private const string AzureGeoCodingQUeryFormat = "/search/address/json?api-version=1.0&query={0}&typeahead=true&limit=50&countrySet=GB&subscription-key={1}";
        
        public SuggestionQueryResult SuggestLocationName(string query)
        {
            var client = new RestClient(AzureGeocodingUrl);

            var request = new RestRequest(string.Format(AzureGeoCodingQUeryFormat, query, WebConfigurationManager.AppSettings["AzureMapsAPIKey"]));
            var response = client.Execute(request);

            dynamic content = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);

            return new SuggestionQueryResult
            {
                Matches = Map(content.results),
            };

        }

        private List<Disambiguation> Map(dynamic results)
        {
            var disambiguationList = new List<Disambiguation>();
            foreach (var result in results)
            {
                if (result.address.countrySubdivision == "ENG")
                {
                    var address = "";
                    if (result.type != "Street" && result.address.postalCode == null && result.address.municipalitySubdivision != null)
                    {
                        address = result.address.municipalitySubdivision + ", " + result.address.municipality;
                    }
                    else
                    {
                        address = result.address.freeformAddress;
                    }

                    address += ", " + result.address.countrySecondarySubdivision;
                    disambiguationList.Add(new Disambiguation
                    {
                        Text = address,
                        LatLon = $"{result.position.lat},{result.position.lon}"                        
                    });
                }
            }

            return disambiguationList;
        }
    }
}