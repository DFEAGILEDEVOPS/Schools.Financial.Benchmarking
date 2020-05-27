using SFB.Web.ApplicationCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFB.Web.Infrastructure.ApiWrappers
{
    public interface IApiRequest
    {
        ApiResponse Get(string endpoint, List<string> actions, Dictionary<string, string> parameters);
        ApiResponse Get(string endpoint, List<string> parameters);
        ApiResponse Head(string endpoint, List<string> parameters);
        Task<ApiResponse> GetAsync (string endpoint, List<string> actions, Dictionary<string, string> parameters);
    }
}
