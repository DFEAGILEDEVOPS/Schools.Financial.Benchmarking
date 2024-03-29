﻿namespace SFB.Web.UI.Helpers.Constants
{
    public class SearchDefaults
    {
        public const int RESULTS_PER_PAGE = 50;
        public const int TRUST_SCHOOLS_PER_PAGE = 200;
        public const int SEARCHED_SCHOOLS_MAX = 1000;
        public const int LINKS_PER_PAGE = 5;
        public const decimal LOCATION_SEARCH_DISTANCE = 3;
        public const decimal TRUST_LOCATION_SEARCH_DISTANCE = 5;
    }

    public class SearchTypes
    {
        public const string SEARCH_BY_NAME_ID = "search-by-name-id";
        public const string SEARCH_BY_LA_ESTAB = "search-by-la-estab";
        public const string SEARCH_BY_LOCATION = "search-by-location";
        public const string SEARCH_BY_LA_CODE_NAME = "search-by-la-code-name";
        public const string SEARCH_BY_TRUST_NAME_ID = "search-by-trust-name-id";
        public const string SEARCH_BY_TRUST_LOCATION = "search-by-trust-location";
        public const string SEARCH_BY_TRUST_LA_CODE_NAME = "search-by-trust-la-code-name";
        public const string SEARCH_BY_MAT = "search-by-mat";
        public const string COMPARE_WITHOUT_DEFAULT_SCHOOL = "compare-without-default-school";
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

    public class TabNames
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
        public const string INTEREST_CHARGES = "Interest charges for loans and banking";
        public const string DIRECT_REVENUE = "Direct revenue financing";
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
        public const string PERCENTAGE_TOTAL_INCOME = "Percentage of total income";
        public const string PERCENTAGE_TOTAL_EXPENDITURE = "Percentage of total expenditure";
        public const string PERCENTAGE_FTE = "Headcount per FTE";
        public const string PERCENTAGE_WORKFORCE = "Percentage of workforce";
        public const string NO_PUPILS_PER_MEASURE = "Pupils per staff role";
    }

    public class CMSelection
    {
        public const string CHART = "Chart view";
        public const string TABLE = "Table view";
    }

    public class BMFinancing
    {
        public const string INCLUDE = "Include share of trust finance";
        public const string EXCLUDE = "Exclude share of trust finance";
    }

    public class CentralFinancing
    {
        public const string INCLUDE = "Include share of trust finance";
        public const string EXCLUDE = "Exclude share of trust finance";
    }

    public class MATFinancing
    {
        public const string TRUST_ONLY = "Trust only";
        public const string TRUST_ACADEMIES = "Trust and academies";
        public const string ACADEMIES_ONLY = "Academies only";
    }

    public class ErrorMessages
    {
        public const string DuplicateSchool = "Select a school which is not already in your list";
        public const string DuplicateTrust = "Select a trust which is not already in your list";
        public const string BMBasketLimitExceed = "Combined basket exceeds 30 schools, either replace current basket or go back and reduce new basket size";
        public const string SelectComparisonType = "Select a comparison type";
        public const string SelectSchoolType = "Select school type";
        public const string SelectAreaType = "Select area type";
        public const string SelectOverwriteStrategy = "Select an option for saving your basket";
    }

    public class HelpTooltipText
    {
        public const string TargetedGrantsHelp = "This includes: pupil premium, education services grants, SEN, funding for minority ethnic pupils, and pupil-focused extended school funding and/or grants.";
        public const string CommunityGrantsHelp = "This includes: academies, non-government grants, community-focused school funding and/or grants, and additional grants for schools.";
        public const string DirectGrantsHelp = "This includes: pre-16 funding, post-16 funding, DfE/EFA revenue grants, other DfE/EFA revenue grants, local authority and other government grants.";
        public const string AdditionalGrantForSchoolsHelp = "This includes: primary PE and sports grants, universal infant free school meal funding, and additional grant funding for secondary schools to release PE teachers to work in primary schools.";
        public const string OccupationChartHelp = "These are costs associated with occupying the school building. They include energy, water, sewerage, rates, insurance, and catering.";
        public const string ShowValueHelp = "<p>This controls the chart value. To change it, select the relevant option from the dropdown. You can choose from the values listed below.</p>" +
            "<ul><li>Headcount per FTE is the number of staff employed per full time position. The higher the figure, the more part time staff and job sharing.</li><li>Percentage of workforce is the percentage of the total workforce the chart grouping represents in the school. For example, teachers might make up 60% of the total school workforce.</li><li>Pupils per staff role is the ratio of pupils to staff for that particular chart group – 22 pupils per total number of teachers, for example.</li></ul>";
        public const string BestInClassHelp = "This allows you to compare your school with the 15 most efficient similar schools, as identified by the Department's <a rel=\"external noopener noreferrer\" target=\"_blank\" href=\"https://www.gov.uk/government/publications/schools-financial-efficiency-metric-tool\">school efficiency metric<span class=\"visuallyhidden\"> Opens in a new window</span></a> tool. <p>These are statistically similar in terms of the proportion of pupils:</p>" +
            "<ul><li>who have been eligible for free school meals in the last 6 years</li><li>with a statement of special educational needs, or an education, health and care (EHC) plan</li></ul>" +
            "<p>The efficiency metric is calculated based on pupil attainment and the money a school receives for its pupils. It is just one indication of efficiency.</p>";
        public const string QuickComparisonHelp = "This generates high-level benchmark charts using pre-selected characteristics (including school phase, number of pupils, SEN, FSM, EAL and per pupil funding). For in-depth benchmarking, we recommend that you do a detailed comparison.</a></p>";
        public const string BicComparisonHelp = "<p>This comparison generates a group of statistically similar schools using the following characteristics and progress data. This gives schools the opportunity to gauge their selected school's progress as well as see statistical data to help them improve their progress.</p>" +
            "<p>The following characteristics and keystage progress data is used:</p>" +
            "<ul><li>Number of pupils</li><li>Eligibility of free school meals</li><li>Total expenditure per pupil</li></ul>";
        public const string QuickComparisonExplanation = "<p>This type of comparison generates a group of statistically similar schools using commonly chosen characteristics.</p>"+
            "<p>The following characteristics are included as standard:</p>"+
            "<ul>"+
            "<li>Number of pupils</li>"+
            "<li>School phase</li>"+
            "<li>Urban/rural schools</li>"+
            "</ul><br>"+
            "<p>The following characteristics can be chosen:</p>"+
            "<ul>"+
            "<li>Pupils eligible for free school meals</li>"+
            "<li>Pupils with special educational needs who have statements or education and health care plans</li>"+
            "<li>Pupils with English as an additional language</li>"+
            "<li>Schools within the local authority</li>"+
            "</ul>";
        public const string SpecialsComparisonExplanation = "<p>This type of comparison generates a group of statistically similar special or alternate provision schools.</p>"+
          "<p>To do this we search nationally for all school funding types and for all size of schools.</p>"+
          "<p>The following characteristics can be chosen:</p>"+
          "<ul>"+
            "<li>Special school, pupil referral unit or both</li>"+
            "<li>Percentage of SEN characteristics</li>"+
          "</ul>";

        public const string DetailedComparisonExplanation = "<p>This type of comparison allows you to tailor a comparison set of schools that match value ranges of school characteristics that you specify. These include:</p>" +
        "<ul>"+
            "<li>General characteristics (such as education phases, pupil size ranges, age ranges, FSM percentages, school types, London weighting, etc.)</li>"+
            "<li>SEN primary need percentage ranges,</li>"+
            "<li>Performance measures, and</li>" +
            "<li>Workforce profiles.</li>"+
        "</ul><br>"+
        "<p>A count of schools matching the criteria selected so far is updated and shown while you select and adjust your criteria. This allows you to tailor a benchmark comparison set of suitable size with schools you know have similar characteristics. </p>";

        public const string ManualComparisonExplanation = "<p>This type of comparison allows you to select specific schools to add to a benchmark set. This can be done by you adding schools by their name, or by adding schools schools from a list of schools found in a radius search near an address you specify or by selecting schools from a list of all schools within a local authority.</ p>" +
            "<p>The location and local authority lists can be filtered down on education phase, school type, Ofsted rating and/or religious character.</p>" +
            "<p>Using this method you can manually select up to 30 schools for a comparison.</p>";

        public const string TrustFinance = "Multi-academy trusts submit finance for each of their academies, they may also submit finance specific to the trust itself.  In order to allow like-for-like comparisons users are able to include or exclude a calculated share of trust finance to each academy within the trust. On this website, we make this calculation on a simple per-pupil basis by dividing the trust’s declared finance by the total number of pupils in the trust that year." +
            "<p>This method makes simple comparisons possible, but it is not necessarily indicative of how allocations are made by trusts in practice.</p>";

        public const string NumberOfPupils = "Pupil numbers for PRU, AP and GHS establishments include all single and dual registered pupils.";
        public const string RevenueReserveExplanation = "<p>For multi-academy trusts the trust is the legal entity and all revenue reserves belong to the trust.We have estimated a value per academy in a MAT by apportioning the trust’s reserves on a pro-rata basis using the FTE number of pupils in each academy within that MAT.</p>" +
            "<p>For local authority maintained schools and single academy trusts revenue reserves are associated to the individual school.</p>";
        public const string QuickComparisonGlossary =   "<p class='govuk-!-margin-top-6'><h3 class='govuk-heading-s'>Staff</h3>This includes teaching staff, supply staff, education support staff, administrative and clerical staff, and other staff costs.</p>" +
                                                        "<p><h3 class='govuk-heading-s'>Premises</h3>This includes premises staff, cleaning and caretaking, maintenance and improvement, and PFI charges.</p>" +
                                                        "<p><h3 class='govuk-heading-s'>Occupation</h3>These are costs associated with occupying the school building. They include energy, water, sewerage, rates, insurance, and catering.</p>" +
                                                        "<p><h3 class='govuk-heading-s'>Supplies and services</h3>This includes administrative supplies, education supples, and bought-in professional services.</p>" +
                                                        "<p><h3 class='govuk-heading-s'>Cost of finance</h3>This includes loan interest and direct revenue financing.</p>" +
                                                        "<p><h3 class='govuk-heading-s'>Special facilities</h3>This includes swimming pools, sports centres, boarding provision, rural units, inter-site travel, before and after school clubs, home to school transport, donation to charity, community education.</p>";
    }

    public class DealsForSchoolsLinkText
    {
        public const string EducationalSupplies = "For information on centrally negotiated deals for books and related materials , including education software";
        public const string Premises = "For information on centrally negotiated deals for facilities management and estates";
        public const string BoughtInProfessionalServices = "For information on centrally negotiated deals for legal and professional services";
        public const string OccupationCosts = "For information on centrally negotiated deals for energy and utilities";
        public const string OtherInsurancePremium = "For information on centrally negotiated deals for insurance and other financial services";
        public const string SupplyStaff = "For information on centrally negotiated deals for HR services including screening services and supply staff";
    }
}