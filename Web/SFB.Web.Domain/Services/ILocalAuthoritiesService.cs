namespace SFB.Web.Domain.Services
{
    public interface ILocalAuthoritiesService
    {
        dynamic GetLocalAuthorities();
        string GetLaName(string laCode);
    }
}
