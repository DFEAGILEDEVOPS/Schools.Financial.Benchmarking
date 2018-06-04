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

        public dynamic GetSchoolByUrn(string urn)
        {
            return _edubaseRepository.GetSchoolByUrn(urn);
        }

        public List<int> GetAllSchoolUrns()
        {
            return _edubaseRepository.GetAllSchoolUrns();
        }

        public bool QuerySchoolByUrn(string urn)
        {
            return _edubaseRepository.QuerySchoolByUrn(urn);
        }

        public dynamic GetSchoolByLaEstab(string laEstab)
        {
            return _edubaseRepository.GetSchoolByLaEstab(laEstab);
        }

        public dynamic GetTrustByName(string name)
        {
            return _edubaseRepository.GetTrustByName(name);
        }

        public dynamic GetMultipleSchoolsByUrns(List<string> urns)
        {
            return _edubaseRepository.GetMultipleSchoolsByUrns(urns);
        }
    }
}