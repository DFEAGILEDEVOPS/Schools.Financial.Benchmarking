using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using SFB.Web.Common;
using SFB.Web.DAL.Helpers;
using SFB.Web.DAL.Repositories;

namespace SFB.Web.Domain.Services.DataAccess
{
    public class FinancialDataService : IFinancialDataService
    {
        private readonly IDataCollectionManager _dataCollectionManager;
        private readonly IFinancialDataRepository _financialDataRepository;

        public FinancialDataService(IDataCollectionManager dataCollectionManager, IFinancialDataRepository financialDataRepository)
        {
            _dataCollectionManager = dataCollectionManager;
            _financialDataRepository = financialDataRepository;
        }
        
        public Task<IEnumerable<Document>> GetSchoolDataDocumentAsync(string urn, string term, SchoolFinancialType schoolFinancialType, CentralFinancingType cFinance)
        {
            return _financialDataRepository.GetSchoolDataDocumentAsync(urn, term, schoolFinancialType, cFinance);
        }

        public Document GetSchoolDataDocument(string urn, string term, SchoolFinancialType schoolFinancialType, CentralFinancingType cFinance)
        {
            return _financialDataRepository.GetSchoolDataDocument(urn, term, schoolFinancialType, cFinance);
        }

        public Document GetMATDataDocument(string matNo, string term, MatFinancingType matFinance)
        {
            return _financialDataRepository.GetMATDataDocument(matNo, term, matFinance);
        }

        public int GetLatestFinancialDataYear()
        {
            return _dataCollectionManager.GetLatestFinancialDataYear();
        }

        public int GetLatestDataYearPerSchoolType(SchoolFinancialType type)
        {
            return _dataCollectionManager.GetLatestFinancialDataYearPerSchoolType(type);
        }

        public List<string> GetActiveTermsForMatCentral()
        {
            return _dataCollectionManager.GetActiveTermsForMatCentral();
        }

        public List<string> GetActiveTermsForAcademies()
        {
            return _dataCollectionManager.GetActiveTermsForAcademies();
        }

        public int GetLatestDataYearForTrusts()
        {
            return _dataCollectionManager.GetLatestFinancialDataYearForTrusts();
        }

        public Task<List<Document>> SearchSchoolsByCriteria(BenchmarkCriteria criteria, EstablishmentType estType)
        {
            return _financialDataRepository.SearchSchoolsByCriteriaAsync(criteria, estType);
        }

        public Task<int> SearchSchoolsCountByCriteria(BenchmarkCriteria criteria, EstablishmentType estType)
        {
            return _financialDataRepository.SearchSchoolsCountByCriteriaAsync(criteria, estType);
        }
     
        public dynamic GetAcademiesByMatNumber(string term, string matNo)
        {
            return _financialDataRepository.GetAcademiesByMatNumber(term, matNo);
        }
    }
}
