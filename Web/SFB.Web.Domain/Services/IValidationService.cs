namespace SFB.Web.Domain.Services
{
    public interface IValidationService
    {
        string ValidateNameParameter(string name);
        string ValidateTrustNameParameter(string name);
        string ValidateLocationParameter(string location);
        string ValidateLaCodeParameter(string laCode);
        string ValidateLaNameParameter(string laName);
        string ValidateSchoolIdParameter(string schoolId);
    }
}
