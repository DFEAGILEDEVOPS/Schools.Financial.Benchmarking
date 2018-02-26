using System.Collections.Generic;
using SFB.Web.DAL.Repositories;

namespace SFB.Web.Domain.Services.DataAccess
{
    public class EdubaseDataService : IEdubaseDataService
    {
        private readonly IEdubaseRepository _edubaseRepository;

        public EdubaseDataService(IEdubaseRepository edubaseRepository)
        {
            _edubaseRepository = edubaseRepository;
        }

        public dynamic GetSchoolByUrn(string urn)
        {
            return _edubaseRepository.GetSchoolByUrn(urn);
        }
        
        public dynamic GetSchoolByLaEstab(string laEstab)
        {
            return _edubaseRepository.GetSchoolByLaEstab(laEstab);
        }

        public dynamic GetSponsorByName(string name)
        {
            return _edubaseRepository.GetSponsorByName(name);
        }

        public dynamic GetMultipleSchoolsByUrns(List<string> urns)
        {
            return _edubaseRepository.GetMultipleSchoolsByUrns(urns);
        }
    }
}