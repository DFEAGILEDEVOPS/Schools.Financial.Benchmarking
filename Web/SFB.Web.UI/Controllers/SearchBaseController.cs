﻿using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Services.Search;
using SFB.Web.UI.Helpers;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFB.Web.UI.Controllers
{
    public abstract class SearchBaseController : Controller
    {
        protected readonly ISchoolSearchService _schoolSearchService;
        protected readonly ITrustSearchService _trustSearchService;
        protected readonly ISchoolBenchmarkListService _schoolBenchmarkListService;
        protected readonly IFilterBuilder _filterBuilder;
        protected readonly IPlacesLookupService _placesLookupService;

        public SearchBaseController(ISchoolSearchService schoolSearchService, ITrustSearchService trustSearchService, ISchoolBenchmarkListService benchmarkListService, IFilterBuilder filterBuilder, IPlacesLookupService placesLookupService)
        {
            _schoolSearchService = schoolSearchService;
            _trustSearchService = trustSearchService;
            _schoolBenchmarkListService = benchmarkListService;
            _filterBuilder = filterBuilder;
            _placesLookupService = placesLookupService;
        }

        protected abstract Task<dynamic> GetSearchResultsAsync(string nameId, string searchType, string locationorpostcode, string locationCoordinates, string laCode, 
            decimal? radius, bool openOnly, string orderby, int page, int take = SearchDefaults.RESULTS_PER_PAGE);
        
        protected void OrderFacetFilters(SearchResultsModel<SchoolSearchResult> results)
        {
            if (results?.Facets != null)
            {
                var orderedFacetFilters = new Dictionary<string, FacetResultModel[]>();
                foreach (var facet in results.Facets)
                {
                    if (facet.Key == "OverallPhase")
                    {
                        orderedFacetFilters.Add(facet.Key, facet.Value.OrderBy(fr => {
                            switch (fr.Value)
                            {
                                case "Nursery":
                                    return 1;
                                case "Primary":
                                    return 2;
                                case "Secondary":
                                    return 3;
                                case "All-through":
                                case "All through":
                                    return 4;
                                case "Pupil referral unit":
                                    return 5;
                                case "Special":
                                    return 6;
                                case "16 plus":
                                    return 7;
                                default:
                                    return 0; ;
                            }
                        }).ToArray());
                    }
                    else
                    {
                        orderedFacetFilters.Add(facet.Key, facet.Value.OrderBy(fr => fr.Value).ToArray());
                    }
                }

                results.Facets = orderedFacetFilters;
            }
        }

        protected string DetermineSelectionState(Models.Filter[] filters)
        {
            bool ofstedExpanded = false, schoolTypeExpanded = false, religiousCharacterExpanded = false, statusExpanded = false;

            if (filters.Where(x => x.Group == "ofstedrating").Any(filter => filter.Metadata.Any(x => x.Checked)))
            {
                ofstedExpanded = true;
            }

            if (filters.Where(x => x.Group == "schooltype").Any(filter => filter.Metadata.Any(x => x.Checked)))
            {
                schoolTypeExpanded = true;
            }

            if (filters.Where(x => x.Group == "faith").Any(filter => filter.Metadata.Any(x => x.Checked)))
            {
                religiousCharacterExpanded = true;
            }

            if (filters.Where(x => x.Group == "establishmentStatus").Any(filter => filter.Metadata.Any(x => x.Checked)))
            {
                statusExpanded = true;
            }

            return
                $"{(ofstedExpanded ? "1" : "0")},{(schoolTypeExpanded ? "1" : "0")},{(religiousCharacterExpanded ? "1" : "0")},{(statusExpanded ? "1" : "0")}";
        }

        protected SearchedSchoolListViewModel GetSearchedSchoolViewModelList(dynamic response, SchoolComparisonListModel schoolComparisonListModel, string orderBy, int page, string searchType, string nameKeyword, string locationKeyword, string laKeyword)
        {
            var schoolListVm = new List<SchoolSearchResultViewModel>();
            var vm = new SearchedSchoolListViewModel(schoolListVm, null, searchType, nameKeyword, locationKeyword, laKeyword, orderBy);
            if (response != null)
            {
                foreach (var result in response.Results)
                {
                    var schoolVm = new SchoolSearchResultViewModel(result);
                    schoolListVm.Add(schoolVm);
                }

                vm.SchoolComparisonList = schoolComparisonListModel;
                                
                _filterBuilder.AddSchoolLevelFilters(response.Facets, Request.QueryString);
                _filterBuilder.AddSchoolTypeFilters(response.Facets, Request.QueryString);
                _filterBuilder.AddOfstedRatingFilters(response.Facets, Request.QueryString);
                _filterBuilder.AddReligiousCharacterFilters(response.Facets, Request.QueryString);
                if (Request.QueryString?["searchType"] == SearchTypes.SEARCH_BY_LOCATION && Request.QueryString?["openOnly"] != "true")
                {
                    _filterBuilder.AddStatusFilters(response.Facets, Request.QueryString);
                }
                var filters = _filterBuilder.GetResult();
                vm.Filters = filters;
                vm.FilterSelectionState = DetermineSelectionState(filters);

                vm.Pagination = new Pagination
                {
                    Start = (SearchDefaults.RESULTS_PER_PAGE * (page - 1)) + 1,
                    Total = response.NumberOfResults,
                    PageLinksPerPage = SearchDefaults.LINKS_PER_PAGE,
                    MaxResultsPerPage = SearchDefaults.RESULTS_PER_PAGE,
                    PagedEntityType = PagedEntityType.School
                };
            }

            return vm;
        }

        protected bool IsNumeric(string field) => field != null ? Regex.IsMatch(field, @"^\d+$") : false;
        protected bool IsLaEstab(string field) => field != null ? Regex.IsMatch(field, "^[0-9]{3}(-|/)?[0-9]{4}$") : false;
        protected bool IsURN(string field) => field != null ? Regex.IsMatch(field, "^[0-9]{5}$") : false;
    }
}