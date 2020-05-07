namespace SFB.Web.ApplicationCore.Services
{
    public interface IValidationService
    {
        string ValidateNameParameter(string name);
        string ValidateTrustNameParameter(string name);
        string ValidateLocationParameter(string location);
        string ValidateLaCodeParameter(string laCode);
        string ValidateLaNameParameter(string laName);
        string ValidateLaCodeNameParameter(string laCodeName);
        string ValidateSchoolIdParameter(string schoolId);
        string ValidateCompanyNoParameter(string companyNo);
    }
}
