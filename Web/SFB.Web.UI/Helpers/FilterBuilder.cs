using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RedDog.Search.Model;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Models;

namespace SFB.Web.UI.Helpers
{
    public class FilterBuilder : IFilterBuilder
    {
        private OptionSelect CreateOption(string displayLabel, string label, string currentlySelected, string facet,
            bool isEnabled)
        {
            var slug = Regex.Replace(label, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
            var labelWithFacet = GetFacetValue(facet, displayLabel);

            return new OptionSelect
            {
                Id = slug,
                Label = labelWithFacet,
                Value = label,
                Disabled = !isEnabled,
                Checked = !string.IsNullOrEmpty(label) && currentlySelected.IndexOf(label, StringComparison.Ordinal) != -1
            };
        }

        private string GetFacetValue(string label, string facet, string facetValue)
        {
            if (_facets == null)
            {
                return label;
            }

            foreach (var facetItem in _facets[facet])
            {
                if (facetItem.Value.ToString().ToLower() == facetValue.ToLower())
                {
                    return $"{label}";
                }
            }

            return $"{label}";
        }

        private string GetFacetValue(string facet, string facetValue)
        {
            return GetFacetValue(facetValue, facet, facetValue);
        }

        private dynamic _facets;

        private string RetrieveParameter(string key, dynamic parameters)
        {
            var value = parameters?[key];
            if (value == null)
            {
                return string.Empty;
            }
            return value;
        }

        public Filter[] ConstructSchoolSearchFilters(dynamic parameters, dynamic facets)
        {
            _facets = facets;
            string schoolLevels = RetrieveParameter("schoollevel", parameters);
            string schoolTypes = RetrieveParameter("schooltype", parameters);
            string ofstedRatings = RetrieveParameter("ofstedrating", parameters);
            string religiousCharacter = RetrieveParameter("faith", parameters);
            string schoolStatus = RetrieveParameter("establishmentStatus", parameters);

            var result = new List<Filter>();
            result.Add(AddSchoolLevelFilters(schoolLevels, facets, parameters));
            result.Add(AddSchoolTypeFilters(schoolTypes, facets, parameters));
            result.Add(AddOfstedRatingFilters(ofstedRatings, facets, parameters));
            result.Add(AddReligiousCharacterFilters(religiousCharacter, facets, parameters));
            if (parameters["searchType"] == SearchTypes.SEARCH_BY_LOCATION && parameters["openOnly"] != "true")
            {
                result.Add(AddStatusFilters(schoolStatus, facets, parameters));
            }
            return result.ToArray();
        }

        private Filter AddStatusFilters(string status, dynamic facets, dynamic queryParams)
        {
            var queryParamForStatus = "establishmentStatus";

            var result = new Filter
            {
                Id = "establishmentStatus",
                Group = "establishmentStatus",
                Label = "Establishment Status"
            };

            var filterSelected = (queryParams?[queryParamForStatus] != null);

            var statusFacets = facets["EstablishmentStatus"];

            var metadata = new List<OptionSelect>();

            foreach (var facet in statusFacets)
            {
                if (facet.Value != string.Empty)
                {
                    metadata.Add(CreateOption(facet.Value, facet.Value, status, "EstablishmentStatus",
                        facet.Count > 0 || filterSelected));
                }
            }

            result.Expanded = metadata.Any(x => x.Checked);
            result.Metadata = metadata.ToArray();

            return result;
        }

        private Filter AddReligiousCharacterFilters(string religiousCharacter, dynamic facets, dynamic queryParams)
        {
            var queryParamForReligiousCharacter = "faith";

            var result = new Filter
            {
                Id = "faith",
                Group = "faith",
                Label = "Religious character"
            };

            var filterSelected = (queryParams?[queryParamForReligiousCharacter] != null);

            var religiousCharacterFacets = facets["ReligiousCharacter"];

            var metadata = new List<OptionSelect>();

            foreach (var facet in religiousCharacterFacets)
            {
                if (facet.Value != string.Empty)
                {
                    metadata.Add(CreateOption(facet.Value, facet.Value, religiousCharacter, "ReligiousCharacter",
                        facet.Count > 0 || filterSelected));
                }
            }

            result.Expanded = metadata.Any(x => x.Checked);
            result.Metadata = metadata.ToArray();

            return result;
        }

        public Filter[] ConstructTrustSchoolSearchFilters(dynamic parameters, dynamic facets)
        {
            _facets = facets;
            string schoolLevels = RetrieveParameter("schoollevel", parameters);
            string ofstedRatings = RetrieveParameter("ofstedrating", parameters);
            string genders = RetrieveParameter("gender", parameters);

            var result = new List<Filter>();
            result.Add(AddSchoolLevelFilters(schoolLevels, facets, parameters));
            result.Add(AddOfstedRatingFilters(ofstedRatings, facets, parameters));
            result.Add(AddGenderFilters(genders, facets, parameters));
            return result.ToArray();
        }

        private Filter AddSchoolTypeFilters(string schoolTypes, dynamic facets, dynamic queryParams)
        {
            var queryParamForSchoolType = "schooltype";

            var result = new Filter
            {
                Id = "schoolType",
                Group = "schooltype",
                Label = "School type"
            };

            var filterSelected = (queryParams?[queryParamForSchoolType] != null);

            var schoolTypeFacets = facets["TypeOfEstablishment"];

            var metadata = new List<OptionSelect>();

            foreach (var facet in schoolTypeFacets)
            {
                if (facet.Value != string.Empty)
                {
                    metadata.Add(CreateOption(facet.Value, facet.Value, schoolTypes, "TypeOfEstablishment",
                        facet.Count > 0 || filterSelected));
                }
            }

            result.Expanded = metadata.Any(x => x.Checked);
            result.Metadata = metadata.ToArray();

            return result;
        }

        private Filter AddSchoolLevelFilters(string schoolLevels, dynamic facets, dynamic queryParams)
        {
            var queryParamForSchoolLevel = "schoollevel";

            var result = new Filter
            {
                Id = "schoolLevel",
                Group = "schoollevel",
                Label = "Education phase"
            };

            var filterSelected = (queryParams?[queryParamForSchoolLevel] != null);

            var schoolLevelFacets = facets["OverallPhase"];

            var metadata = new List<OptionSelect>();

            foreach (var facet in schoolLevelFacets)
            {
                if (facet.Value != string.Empty)
                {
                    metadata.Add(CreateOption(facet.Value, facet.Value, schoolLevels, "OverallPhase",
                        facet.Count > 0 || filterSelected));
                }
            }

            result.Expanded = true;
            result.Metadata = metadata.ToArray();

            return result;
        }

        private Filter AddGenderFilters(string schoolLevels, dynamic facets, dynamic queryParams)
        {
            var queryParamForSchoolLevel = "gender";

            var result = new Filter
            {
                Id = "gender",
                Group = "gender",
                Label = "Pupil gender"
            };

            var filterSelected = (queryParams?[queryParamForSchoolLevel] != null);

            var schoolLevelFacets = facets["Gender"];

            var metadata = new List<OptionSelect>();

            foreach (var facet in schoolLevelFacets)
            {
                if (facet.Value != string.Empty)
                {
                    metadata.Add(CreateOption(facet.Value, facet.Value, schoolLevels, "Gender",
                        facet.Count > 0 || filterSelected));
                }
            }

            result.Expanded = false;
            result.Metadata = metadata.ToArray();

            return result;
        }

        private Filter AddOfstedRatingFilters(string ofstedRatings, dynamic facets, dynamic queryParams)
        {
            var queryParamForOfstedRating = "ofstedrating";

            var result = new Filter
            {
                Id = "ofstedrating",
                Group = "ofstedrating",
                Label = "Ofsted rating"
            };

            var filterSelected = (queryParams?[queryParamForOfstedRating] != null);

            var ofstedRatingFacets = facets["OfstedRating"];
            var ofstedRatingFacetsOrdered = ((FacetResult[]) ofstedRatingFacets).ToList().OrderBy(o => o.Value);
            var metadata = new List<OptionSelect>();

            foreach (var facet in ofstedRatingFacetsOrdered)
            {
                if (facet.Value != "NULL")
                {
                    metadata.Add(CreateOption(ConvertOfstedRating(facet.Value), facet.Value, ofstedRatings,
                        "OfstedRating", facet.Count > 0 || filterSelected));
                }
            }

            result.Expanded = metadata.Any(x => x.Checked);
            result.Metadata = metadata.ToArray();

            return result;
        }

        private string ConvertOfstedRating(string value)
        {
            string result = string.Empty;
            switch (value)
            {
                case "1":
                    result = OfstedRatings.Description.OUTSTANDING;
                    break;
                case "2":
                    result = OfstedRatings.Description.GOOD;
                    break;
                case "3":
                    result = OfstedRatings.Description.REQUIRES_IMPROVEMENT;
                    break;
                case "4":
                    result = OfstedRatings.Description.INADEQUATE;
                    break;
                default:
                    result = OfstedRatings.Description.NOT_RATED;
                    break;
            }

            return result;
        }
    }
}