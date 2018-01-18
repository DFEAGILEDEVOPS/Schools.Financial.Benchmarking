﻿namespace SFB.Web.Domain.Helpers.Constants
{
    public class SearchErrorMessages
    {
        // where min values are specified in the message, ensure they're consistent with the corresponding SearchParameterValidLength
        public const string NAME_ERR_MESSAGE =
            "Please enter the name of the school or college you're looking for (minimum 3 characters)";

        public const string TRUST_NAME_ERR_MESSAGE =
            "Please enter the name of the trust you're looking for (minimum 3 characters)";

        public const string LOCATION_ERR_MESSAGE = "Please enter the name of a town or city (minimum 2 characters)";
        public const string LA_CODE_ERR_MESSAGE = "Please enter the LA code for the local authorithy (3 characters)";

        public const string LA_NAME_ERR_MESSAGE =
            "Please enter the name of the local authority you're looking for (minimum 2 characters)";

        public const string SCHOOL_ID_ERR_MESSAGE =
            "Please enter the URN code (6 characters) or LAESTAB code (7 characters) of the school or college you're looking for";
    }

    public class SearchParameterValidLengths
    {
        // check these values correspond to any reference to their values in SearchErrorMessages
        public const int NAME_MIN_LENGTH = 3;
        public const int LOCATION_MIN_LENGTH = 2;
        public const int LA_CODE_LENGTH = 3;
        public const int LA_NAME_MIN_LENGTH = 2;
        public const int URN_LENGTH = 6;
        public const int LAESTAB_LENGTH = 7;
    }

    public class SearchTypes
    {
        public const string SEARCH_BY_NAME_ID = "search-by-name-id";
        public const string SEARCH_BY_TRUST_NAME = "search-by-trust-name";
        public const string SEARCH_BY_LOCATION = "search-by-location";
        public const string SEARCH_BY_LA_CODE_NAME = "search-by-la-code-name";
    }

    public class SearchDefaults
    {
        public const int RESULTS_PER_PAGE = 50;
        public const int TRUST_SCHOOLS_PER_PAGE = 200;
        public const int LINKS_PER_PAGE = 5;
        public const decimal LOCATION_SEARCH_DISTANCE = 3;
    }

    public struct DataGroups
    {
        public const string Academies = "Academies";
        public const string Edubase = "Edubase";
        public const string Maintained = "Maintained";
        public const string MATCentral = "MAT-Central";
        public const string MATOverview = "MAT-Overview";
        public const string MATDistributed = "MAT-Distributed";
        public const string Unidentified = "Unidentified";
    }

    public class CriteriaFieldComparisonTypes
    {
        public const string MAX = "Max";
        public const string MIN = "Min";
        public const string EQUALTO = "Eq";
    }

    public class SchoolCharacteristicsQuestions
    {
        public const string LA_CODE = "Local Authority Code";
        public const string NUMBER_OF_PUPILS = "Number of pupils";
        public const string GENDER_OF_PUPILS = "Gender of pupils";
        public const string SCHOOL_PHASE = "School phase";
        public const string SCHOOL_OVERALL_PHASE = "School overall phase";
        public const string TYPEOF_ESTABLISHMENT = "School type";
        public const string URBAN_RURAL = "Urban/rural schools";
        public const string GOVERNMENT_OFFICE = "Government office region";
        public const string LONDON_BOROUGH = "London borough";
        public const string LONDON_WEIGHTING = "London weighting";
        public const string PERCENTAGE_OF_ELIGIBLE_FREE_SCHOOL_MEALS = "Eligibility for free school meals";
        public const string PERCENTAGE_OF_PUPILS_WITH_STATEMENT_OF_SEN = "Pupils with SEN who have statements or EHC plans";
        public const string PERCENTAGE_OF_PUPILS_ON_SEN_REGISTER = "Pupils with special educational needs (SEN) who don't have statements or education and health care (EHC) plans";
        public const string PERCENTAGE_OF_PUPILS_WITH_EAL = "Pupils with English as an additional language";
        public const string PERCENTAGE_BOARDERS = "Boarders";
        public const string ADMISSIONS_POLICY = "Admissions policy";
        public const string PFI = "Part of a private finance initiative?";
        public const string DOES_THE_SCHOOL_HAVE6_FORM = "Does the school have a sixth form?";
        public const string NUMBER_IN6_FORM = "Number in sixth form";
        public const string LOWEST_AGE_PUPILS = "Lowest age of pupils";
        public const string HIGHEST_AGE_PUPILS = "Highest age of pupils";
        public const string PERCENTAGE_QUALIFIED_TEACHERS = "Percentage of teachers with qualified teacher status (full time equivalent)";
        public const string FULL_TIME_TA = "Number of teaching assistants (full time equivalent)";
        public const string FULL_TIME_OTHER = "Number of non-classroom support staff – excluding auxiliary staff (full time equivalent)";
        public const string FULL_TIME_ADMIN = "Number of admin/clerical staff (full time equivalent)";
        public const string SCHOOL_WORKFORCE_FTE = "Number in the school workforce (full time equivalent)";
        public const string NUMBER_OF_TEACHERS_FTE = "Number of teachers (full time equivalent)";
        public const string SENIOR_LEADERSHIP_FTE = "Number in the senior leadership team (full time equivalent)";
        public const string OFSTED_RATING = "Ofsted rating";
        public const string KS2_ACTUAL = "KS2 attainment";
        public const string KS2_PROGRESS = "KS2 progress";
        public const string AVERAGE_ATTAINMENT_8 = "Average attainment 8";
        public const string PROGRESS_8_MEASURE = "Progress 8 measure";
        public const string SPECIFIC_LEARNING_DIFFICULTY = "Specific learning difficulty";
        public const string MODERATE_LEARNING_DIFFICULTY = "Moderate learning difficulty";
        public const string SEVERE_LEARNING_DIFFICULTY = "Severe learning difficulty";
        public const string PROF_LEARNING_DIFFICULTY = "Profound and multiple learning difficulty";
        public const string SOCIAL_HEALTH = "Social, emotional and mental health";
        public const string SPEECH_NEEDS = "Speech, language and communications needs";
        public const string HEARING_IMPAIRMENT = "Hearing impairment";
        public const string VISUAL_IMPAIRMENT = "Visual impairment";
        public const string MULTI_SENSORY_IMPAIRMENT = "Multi-sensory impairment";
        public const string PHYSICAL_DISABILITY = "Physical disability";
        public const string AUTISTIC_DISORDER = "Autistic spectrum disorder";
        public const string OTHER_LEARNING_DIFF = "Other learning difficulty";
    }

    public class DBFieldNames
    {
        public const string KS2_PROGRESS = "KS2 progress";
        public const string PROGRESS_8_MEASURE = "P8MEA";
        public const string AVERAGE_ATTAINMENT = "ATT8SCR";
        public const string OFSTED_RATING = "OfstedRating";
        public const string OFSTED_RATING_NAME = "OfstedRatingName";
        public const string KS2_ACTUAL = "PTRWM_EXP";
        public const string TEACHERS_LEADER = "TotalNumberOfTeachersInTheLeadershipGroupFullTimeEquivalent";
        public const string TEACHERS_TOTAL = "TotalNumberOfTeachersFullTimeEquivalent";
        public const string WORKFORCE_TOTAL = "TotalSchoolWorkforceFullTimeEquivalent";
        public const string WORKFORCE_HEADCOUNT = "TotalSchoolWorkforceHeadcount";
        public const string AUX_STAFF = "TotalNumberOfAuxiliaryStaffFullTimeEquivalent";
        public const string ADMIN_STAFF = "FTE of Admin Staff";
        public const string FULL_TIME_OTHER = "TotalNumberOfNonClassroomBasedSchoolSupportStaffExcludingAuxiliaryStaffFullTimeEquivalent";
        public const string FULL_TIME_TA = "TotalNumberOfTeachingAssistantsFullTimeEquivalent";
        public const string PERCENTAGE_QUALIFIED_TEACHERS = "TeachersWithQualifiedTeacherStatus";
        public const string HIGHEST_AGE_PUPILS = "Highest age of pupils";
        public const string LOWEST_AGE_PUPILS = "Lowest age of pupils";
        public const string NUMBER_IN_6_FORM = "No of pupils in 6th form";
        public const string ADMISSION_POLICY = "Admissions policy";
        public const string PERCENTAGE_BOARDERS = "% of pupils who are Boarders";
        public const string PERCENTAGE_OF_PUPILS_WITH_EAL = "% of pupils with EAL";
        public const string PERCENTAGE_OF_PUPILS_WITHOUT_SEN = "% of pupils without SEN Statement";
        public const string PERCENTAGE_OF_PUPILS_WITH_SEN = "% of pupils with SEN Statement";
        public const string PERCENTAGE_FSM = "% of pupils eligible for FSM";
        public const string NO_PUPILS = "No Pupils";
        public const string LONDON_WEIGHT = "London Weighting";
        public const string LONDON_BOROUGH = "London Borough";
        public const string REGION = "Region";
        public const string URBAN_RURAL = "UrbanRuralInner";
        public const string SCHOOL_TYPE = "Type";
        public const string SCHOOL_PHASE = "Phase";
        public const string SCHOOL_OVERALL_PHASE = "Overall Phase";
        public const string GENDER = "Gender";
        public const string HAS_6_FORM = "Has a 6th form";
        public const string PFI = "PFI";
        public const string LA_CODE = "LA";
        public const string SPECIFIC_LEARNING_DIFFICULTY = "Primary_need_spld_percent";
        public const string MODERATE_LEARNING_DIFFICULTY = "Primary_need_mld_percent";
        public const string SEVERE_LEARNING_DIFFICULTY = "Primary_need_sld_percent";
        public const string PROF_LEARNING_DIFFICULTY = "Primary_need_pmld_percent";
        public const string SOCIAL_HEALTH = "Primary_need_semh_percent";
        public const string SPEECH_NEEDS = "Primary_need_slcn_percent";
        public const string HEARING_IMPAIRMENT = "Primary_need_hi_percent";
        public const string VISUAL_IMPAIRMENT = "Primary_need_vi_percent";
        public const string MULTI_SENSORY_IMPAIRMENT = "Primary_need_msi_percent";
        public const string PHYSICAL_DISABILITY = "Primary_need_pd_percent";
        public const string AUTISTIC_DISORDER = "Primary_need_asd_percent";
        public const string OTHER_LEARNING_DIFF = "Primary_need_oth_percent";
    }
}