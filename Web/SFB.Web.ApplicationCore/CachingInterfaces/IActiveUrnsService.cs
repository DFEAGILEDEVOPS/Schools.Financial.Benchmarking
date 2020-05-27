using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFB.Web.ApplicationCore.Services
{
    public interface IActiveUrnsService
    {
        Task<List<int>> GetAllActiveUrnsAsync();
    }
}
