using System.Collections.Generic;
using SFB.Web.DAL.Repositories;

namespace SFB.Web.Domain.Services.DataAccess
{
    public class ContextDataService : IContextDataService
    {
        private readonly IEdubaseRepository _edubaseRepository;

        public ContextDataService(IEdubaseRepository edubaseRepository)
        {
            _edubaseRepository = edubaseRepository;
        }

        public dynamic GetSchoolByUrn(int urn)
        {
            return _edubaseRepository.GetSchoolByUrn(urn);
        }

        public List<int> GetAllSchoolUrns()
        {
            return _edubaseRepository.GetAllSchoolUrns();
        }

        public dynamic GetSchoolByLaEstab(string laEstab)
        {
            return _edubaseRepository.GetSchoolByLaEstab(laEstab);
        }

        public dynamic GetMultipleSchoolsByUrns(List<int> urns)
        {
            return _edubaseRepository.GetMultipleSchoolsByUrns(urns);
        }
    }
}