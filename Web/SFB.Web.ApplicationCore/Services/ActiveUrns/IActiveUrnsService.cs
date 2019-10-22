using System.Collections.Generic;

namespace SFB.Web.ApplicationCore.Services
{
    public interface IActiveUrnsService
    {
        List<int> GetAllActiveUrns();
    }
}
