using SFB.Web.ApplicationCore.Helpers.Constants;
using System.Text.RegularExpressions;

namespace SFB.Web.ApplicationCore.Services
{
    public class ValidationService : IValidationService
    {
        public string ValidateNameParameter(string name)
        {
            if (name == null || name.Length < SearchParameterValidLengths.NAME_MIN_LENGTH)
            {
                return SearchErrorMessages.NAME_ERR_MESSAGE;
            }

            return null;
        }

        public string ValidateTrustNameParameter(string name)
        {
            if (name == null || name.Length < SearchParameterValidLengths.NAME_MIN_LENGTH)
            {
                return SearchErrorMessages.TRUST_NAME_ERR_MESSAGE;
            }

            return null;
        }

        public string ValidateLocationParameter(string location)
        {
            if (location == null || location.Length < SearchParameterValidLengths.LOCATION_MIN_LENGTH)
            {
                return SearchErrorMessages.LOCATION_ERR_MESSAGE;
            }

            return null;
        }

        public string ValidateLaCodeParameter(string laCode)
        {
            if (laCode == null || laCode.Length != SearchParameterValidLengths.LA_CODE_LENGTH)
            {
                return SearchErrorMessages.LA_CODE_ERR_MESSAGE;
            }

            return null;
        }

        public string ValidateLaNameParameter(string laName)
        {
            if (laName == null || laName.Length < SearchParameterValidLengths.LA_NAME_MIN_LENGTH)
            {
                return SearchErrorMessages.LA_NAME_ERR_MESSAGE;
            }

            return null;
        }

        public string ValidateLaCodeNameParameter(string laCodeName)
        {
            if ((IsNumeric(laCodeName) && laCodeName.Length != SearchParameterValidLengths.LA_CODE_LENGTH)
                || (!IsNumeric(laCodeName) && laCodeName.Length < SearchParameterValidLengths.LA_NAME_MIN_LENGTH))
            {
                return SearchErrorMessages.LA_CODE_NAME_ERR_MESSAGE;
            }

            return null;
        }

        public string ValidateSchoolIdParameter(string schoolId)
        {
            if (schoolId == null || (schoolId.Length != SearchParameterValidLengths.URN_LENGTH && schoolId.Length != SearchParameterValidLengths.LAESTAB_LENGTH))
            {
                return SearchErrorMessages.SCHOOL_ID_ERR_MESSAGE;
            }

            return null;
        }

        public string ValidateCompanyNoParameter(string companyNo)
        {
            int result;
            var isNumber = int.TryParse(companyNo, out result);
            if (!isNumber || companyNo.Length < SearchParameterValidLengths.COMPANY_NO_LENGTH_MIN || companyNo.Length > SearchParameterValidLengths.COMPANY_NO_LENGTH_MAX)
            {
                return SearchErrorMessages.COMPANY_NO_ERR_MESSAGE;
            }

            return null;
        }

        private bool IsNumeric(string field) => field != null ? Regex.IsMatch(field, @"^\d+$") : false;
    }
}
