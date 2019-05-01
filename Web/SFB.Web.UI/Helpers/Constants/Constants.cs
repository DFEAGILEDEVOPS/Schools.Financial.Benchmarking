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
        public const string SEARCH_BY_TRUST_NAME_ID = "search-by-trust-name-id";
        public const string SEARCH_BY_LOCATION = "search-by-location";
        public const string SEARCH_BY_LA_CODE_NAME = "search-by-la-code-name";
    }

    public class ChartHistory
    {
        public const int YEARS_OF_HISTORY = 5;
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

    public class CMSelection
    {
        public const string CHART = "Chart view";
        public const string TABLE = "Table view";
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
        public const string BMBasketLimitExceed = "Combined basket exceeds 30 schools, either replace current basket or go back and reduce new basket size";
    }

    public class MoreInfoText
    {
        public const string INTERPRET_CHARTS =
            "For more information about this chart, read our guidance on <a href=\"../Help/InterpretingCharts\">Interpreting the data</a>.";
    }

    public class HelpTooltipText
    {
        public const string TargetedGrantsHelp = "This includes: pupil premium, education services grants, SEN, funding for minority ethnic pupils, and pupil-focused extended school funding and/or grants.";
        public const string CommunityGrantsHelp = "This includes: academies, non-government grants, community-focused school funding and/or grants, and additional grants for schools.";
        public const string DirectGrantsHelp = "This includes: pre-16 funding, post-16 funding, DfE/EFA revenue grants, other DfE/EFA revenue grants, local authority and other government grants.";
        public const string AdditionalGrantForSchoolsHelp = "This includes: primary PE and sports grants, universal infant free school meal funding, and additional grant funding for secondary schools to release PE teachers to work in primary schools.";
        public const string OccupationChartHelp = "These are costs associated with occupying the school building. They include energy, water, sewerage, rates, insurance, and catering.";
        public const string ShowValueHelp = "<p>This controls the chart value. To change it, select the relevant option from the dropdown. You can choose from the values listed below.</p>" +
            "<ul><li>Headcount per FTE is the number of staff employed per full time position. The higher the figure, the more part time staff and job sharing.</li><li>Percentage of workforce is the percentage of the total workforce the chart grouping represents in the school. For example, teachers might make up 60% of the total school workforce.</li><li>Pupils per measure is the ratio of pupils to staff for that particular chart group – 22 pupils per total number of teachers, for example.</li></ul>";
        public const string BestInClassHelp = "This allows you to compare your school with the 15 most efficient similar schools, as identified by the Department's <a rel=\"external noopener noreferrer\" target=\"_blank\" href=\"https://www.gov.uk/government/publications/schools-financial-efficiency-metric-tool\">school efficiency metric<span class=\"visuallyhidden\"> Opens in a new window</span></a> tool. <p>These are statistically similar in terms of the proportion of pupils:</p>" +
            "<ul><li>who have been eligible for free school meals in the last 6 years</li><li>with a statement of special educational needs, or an education, health and care (EHC) plan</li></ul>" +
            "<p>The efficiency metric is calculated based on pupil attainment and the money a school receives for its pupils. It is just one indication of efficiency.</p>";
        public const string QuickComparisonHelp = "This generates high-level benchmark charts using pre-selected characteristics (including school phase, number of pupils, SEN, FSM, EAL and per pupil funding). For in-depth benchmarking, we recommend that you do a detailed comparison.</a></p>";
        public const string BicComparisonHelp = "<p>This comparison generates a group of statistically similar schools using the following characteristics and progress data. This gives schools the opportunity to gauge their selected school's progress as well as see statistical data to help them improve their progress.</p>" +
            "<p>The following characteristics and keystage progress data is used:</p>" +
            "<ul><li>Number of pupils</li><li>Eligibility of free school meals</li><li>Total expenditure per pupil</li></ul>";
    }
}