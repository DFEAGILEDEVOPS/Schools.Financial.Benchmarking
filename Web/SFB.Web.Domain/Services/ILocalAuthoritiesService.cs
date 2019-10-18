namespace SFB.Web.ApplicationCore.Services
{
    public interface ILocalAuthoritiesService
    {
        dynamic GetLocalAuthorities();
        string GetLaName(string laCode);
    }
}
