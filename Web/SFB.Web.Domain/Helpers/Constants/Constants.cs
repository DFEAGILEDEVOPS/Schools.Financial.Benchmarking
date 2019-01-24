namespace SFB.Web.Domain.Helpers.Constants
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

        public const string COMPANY_NO_ERR_MESSAGE =
            "Please enter the company number (7 characters) of the trust you're looking for";
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
        public const int COMPANY_NO_LENGTH = 7;
    }

    public class ComparisonListLimit
    {
        public const int LIMIT = 30;
        public const int MAT_LIMIT = 20;
        public const int DEFAULT = 15;
        public const int ONE_CLICK = 15;
    }

    public class CriteriaSearchConfig
    {
        public const int DEFAULT_MARGIN = 10;
        public const int MAX_TRY_LIMIT = 10;
        public const decimal BIC_DEFAULT_FLEX = 0.15M;
    }
}
