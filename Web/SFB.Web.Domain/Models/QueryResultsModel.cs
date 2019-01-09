﻿using RedDog.Search.Model;
using System;
using System.Collections.Generic;

namespace SFB.Web.Domain.Models
{
    public class QueryResultsModel
    {
        public QueryResultsModel(int numberOfResults, Dictionary<string, FacetResult[]> facets, IEnumerable<IDictionary<string, object>> results,
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
        public IEnumerable<IDictionary<string, object>> Results { get; set; }
        public int Taken { get; private set; }
        public int Skipped { get; private set; }
        public Dictionary<string, FacetResult[]> Facets { get; set; }
        public string QueryLat { get; set; }
        public string QueryLong { get; set; }
        public bool Disambiguate { get; set; }
    }

    public class SuggestionQueryResult
    {
        public List<Disambiguation> Matches { get; set; }

        public SuggestionQueryResult(List<Disambiguation> matches)
        {
            Matches = matches;
        }

        public SuggestionQueryResult()
        {

        }
    }

    public class Disambiguation
    {
        public string Text { get; set; }
        public string LatLon { get; set; }
    }
}
