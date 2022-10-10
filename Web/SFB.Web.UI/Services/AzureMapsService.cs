using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Models;

namespace SFB.Web.UI.Services
{
    public class AzureMapsService : IAzureMapsService
    {
        private static readonly string _apiKey = ConfigurationManager.AppSettings["AzureMapsAPIKey"];

        private static readonly HttpClient _azureMapsClient = new HttpClient
        {
            BaseAddress = new Uri("https://atlas.microsoft.com")
        };

        private static readonly AsyncRetryPolicy RetryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(4)
            });


        public async Task<PlaceDto[]> SearchAsync(string text, bool isTypeahead)
        {
            text = text.Clean();
            if (string.IsNullOrWhiteSpace(text))
            {
                return new PlaceDto[0];
            }

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/search/address/json?api-version=1.0&countrySet=GB&typeahead={(isTypeahead ? "true" : "false")}&limit=10&query={text}&subscription-key={_apiKey}");

            using (var response = await RetryPolicy.ExecuteAsync(async () =>
                       await _azureMapsClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)))
            {
                var stream = await response.Content.ReadAsStreamAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new PlaceDto[0];
                }

                using (var sr = new StreamReader(stream))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    var serializer = new JsonSerializer();
                    var azureMapsResponse = serializer.Deserialize<AzureMapsSearchResponseDto>(reader);
                    var results = azureMapsResponse.results
                        .Where(result => result.type != "Cross street" &&
                                         !(result.entityType != null &&
                                           result.entityType == "CountrySecondarySubdivision"))
                        .ToList();
                    
                    var municipalities = results.Where(x => x.entityType == "Municipality").ToList();
                    var subMunicipalities = results.Where(x => x.entityType == "MunicipalitySubdivision").ToList();
                    // If the response contains a "MunicipalitySubdivision" with the same name as a returned Municipality (town),
                    // use the coordinates of that result for the position of the town and remove it from the result set.
                    // This addresses an issue where a small number of towns have inaccurate coordinates associated with them.
                    foreach (var municipality in municipalities)
                    {
                        var child = subMunicipalities.FirstOrDefault(
                            x => x.address.municipality == municipality.address.municipality
                                 && x.address.municipalitySubdivision == municipality.address.municipality);
                        if (child == null)
                        {
                            continue;
                        }
                        municipality.position.lat = child.position.lat;
                        municipality.position.lon = child.position.lon;
                        results.Remove(child);
                    }

                    return results.Select(x => new PlaceDto(
                        x.id, 
                        GetAddressDescription(x, text), 
                        new LatLon(x.position.lat, x.position.lon))).ToArray();
                }
            }
        }
        private static string GetAddressDescription(Result locationResult, string text)
        {
            var output = "";

            if (locationResult.entityType != null && locationResult.entityType.ToString() == "MunicipalitySubdivision")
            {
                output = $"{locationResult.address.municipalitySubdivision}, {locationResult.address.municipality}";
            }
            else
            {
                output = locationResult.address.freeformAddress;
            }

            // if a location shares multiple postcodes, azure does not include it within the normal freeformaddress. So we need to build the appropriate address.
            if (locationResult.address.postalCode != null && !output.Contains(locationResult.address.postalCode.Split(',')[0]))
            {
                if (text.IsUkPostCode())
                {
                    if (locationResult.address.extendedPostalCode.ToLower().Remove(" ").Contains(text.ToLower().Remove(" ")))
                    {
                        output += $", {text.ToUpper()}";
                    }
                    else
                    {
                        output += $", {locationResult.address.postalCode.Split(',')[0]}";
                    }
                }
            }

            return $"{output}, {locationResult.address.countrySecondarySubdivision}";
        }

    }
}