using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Models;

namespace SFB.Web.UI.Helpers
{
    public class FilterBuilder : IFilterBuilder
    {
        private dynamic _facets;
        private List<Filter> _result;

        public FilterBuilder()
        {
            _result = new List<Filter>();
        }

        public Filter[] GetResult()
        {
            return _result.ToArray();
        }

        public void AddStatusFilters(dynamic facets, dynamic queryParams)
        {
            _facets = facets;
            string status = RetrieveParameter("establishmentStatus", queryParams);

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

            _result.Add(result);
        }

        public void AddReligiousCharacterFilters(dynamic facets, dynamic queryParams)
        {
            _facets = facets;
            string religiousCharacter = RetrieveParameter("faith", queryParams);

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

            _result.Add(result);
        }

        public void AddSchoolLevelFilters(dynamic facets, dynamic queryParams)
        {
            _facets = facets;
            string schoolLevels = RetrieveParameter("schoollevel", queryParams);

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

            _result.Add(result);
        }

        public void AddSchoolTypeFilters(dynamic facets, dynamic queryParams)
        {
            _facets = facets;
            string schoolTypes = RetrieveParameter("schooltype", queryParams);

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

            _result.Add(result);
        }

        public void AddGenderFilters(dynamic facets, dynamic queryParams)
        {
            _facets = facets;

            string genders = RetrieveParameter("gender", queryParams);

            var queryParamForGender = "gender";

            var result = new Filter
            {
                Id = "gender",
                Group = "gender",
                Label = "Pupil gender"
            };

            var filterSelected = (queryParams?[queryParamForGender] != null);

            var schoolLevelFacets = facets["Gender"];

            var metadata = new List<OptionSelect>();

            foreach (var facet in schoolLevelFacets)
            {
                if (facet.Value != string.Empty)
                {
                    metadata.Add(CreateOption(facet.Value, facet.Value, genders, "Gender",
                        facet.Count > 0 || filterSelected));
                }
            }

            result.Expanded = false;
            result.Metadata = metadata.ToArray();

            _result.Add(result);
        }

        public void AddOfstedRatingFilters(dynamic facets, dynamic queryParams)
        {
            _facets = facets;
            string ofstedRatings = RetrieveParameter("ofstedrating", queryParams);
            var queryParamForOfstedRating = "ofstedrating";

            var result = new Filter
            {
                Id = "ofstedrating",
                Group = "ofstedrating",
                Label = "Ofsted rating"
            };

            var filterSelected = (queryParams?[queryParamForOfstedRating] != null);

            var ofstedRatingFacets = facets["OfstedRating"];
            var ofstedRatingFacetsOrdered = ((FacetResultModel[]) ofstedRatingFacets).ToList().OrderBy(o => o.Value);
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

            _result.Add(result);
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

        private OptionSelect CreateOption(string displayLabel, string label, string currentlySelected, string facet, bool isEnabled)
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

        private string RetrieveParameter(string key, dynamic parameters)
        {
            var value = parameters?[key];
            if (value == null)
            {
                return string.Empty;
            }
            return value;
        }
    }
}