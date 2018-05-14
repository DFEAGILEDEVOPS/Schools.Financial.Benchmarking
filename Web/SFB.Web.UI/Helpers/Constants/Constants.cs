namespace SFB.Web.UI.Helpers.Constants
{
    public class SearchDefaults
    {
        public const int RESULTS_PER_PAGE = 50;
        public const int TRUST_SCHOOLS_PER_PAGE = 200;
        public const int LINKS_PER_PAGE = 5;
        public const decimal LOCATION_SEARCH_DISTANCE = 3;
    }

    public class SearchTypes
    {
        public const string SEARCH_BY_NAME_ID = "search-by-name-id";
        public const string SEARCH_BY_TRUST_NAME = "search-by-trust-name";
        public const string SEARCH_BY_LOCATION = "search-by-location";
        public const string SEARCH_BY_LA_CODE_NAME = "search-by-la-code-name";
    }

    public class ChartHistory
    {
        public const int YEARS_OF_HISTORY = 5;
    }

    public class CookieNames
    {
        public const string COMPARISON_LIST = "sfb_comparison_list";
        public const string COMPARISON_LIST_MAT = "sfb_comparison_list_mat";
    }

    public class CompareActions
    {
        public const string ADD_TO_COMPARISON_LIST = "addtocompare";
        public const string REMOVE_FROM_COMPARISON_LIST = "removefromcompare";
        public const string MAKE_DEFAULT_BENCHMARK = "makedefaultbenchmark";
        public const string REMOVE_DEFAULT_BENCHMARK = "removedefaultbenchmark";
        public const string CLEAR_BENCHMARK_LIST = "clear";
    }

    public class OfstedRatings
    {
        public struct Rating
        {
            public const string OUTSTANDING = "1";
            public const string GOOD = "2";
            public const string REQUIRES_IMPROVEMENT = "3";
            public const string INADEQUATE = "4";
        }

        public struct Description
        {
            public const string OUTSTANDING = "Outstanding";
            public const string GOOD = "Good";
            public const string REQUIRES_IMPROVEMENT = "Requires improvement";
            public const string INADEQUATE = "Inadequate";
            public const string NOT_RATED = "Not rated";
        }
    }

    public class RevenueGroupNames
    {
        public const string INCOME = "Income";
        public const string EXPENDITURE = "Expenditure";
        public const string BALANCE = "Balance";
        public const string WORKFORCE = "Workforce";
        public const string CUSTOM = "Your charts";
    }

    public class ChartGroupNames
    {
        public const string GRANT_FUNDING = "Grant funding";
        public const string SELF_GENERATED = "Self-generated";
        public const string STAFF = "Staff";
        public const string PREMISES = "Premises";
        public const string OCCUPATION = "Occupation";
        public const string SUPPLIES_AND_SERVICES = "Supplies and services";
        public const string COST_OF_FINANCE = "Cost of finance";
        public const string COMMUNITY = "Community";
        public const string OTHER = "Other";
        public const string SPECIAL_FACILITIES = "Special facilities";
        public const string IN_YEAR_BALANCE = "In-year balance";
        public const string TOTAL_INCOME = "Total income";
        public const string TOTAL_EXPENDITURE = "Total expenditure";
    }

    public class ChartShowValues
    {
        public const string ABSOLUTE = "Absolute total";
        public const string TOTAL = "Total";
        public const string PER_TEACHER = "Per teacher";
        public const string PER_PUPIL = "Per pupil";
        public const string PERCENTAGE_TOTAL = "Percentage of total";
        public const string PERCENTAGE_FTE = "Headcount per FTE";
        public const string PERCENTAGE_WORKFORCE = "Percentage of workforce";
        public const string NO_PUPILS_PER_MEASURE = "Pupils per measure";
    }

    public class BMFinancing
    {
        public const string INCLUDE = "Include MAT central finance";
        public const string EXCLUDE = "Exclude MAT central finance";
    }

    public class CentralFinancing
    {
        public const string INCLUDE = "Academy and trust proportion";
        public const string EXCLUDE = "Academy only";
    }

    public class MATFinancing
    {
        public const string TRUST_ONLY = "Trust only";
        public const string TRUST_ACADEMIES = "Trust and academies";
        public const string ACADEMIES_ONLY = "Academies only";
    }

    public class ErrorMessages
    {
        public const string DuplicateTrust = "Please select a trust which is not already in your list";
    }

    public class MoreInfoText
    {
        public const string INTERPRET_CHARTS =
            "For more information about this chart, read our guidance on <a href=\"../Help/InterpretingCharts\">Interpreting the charts</a>.";
    }
}