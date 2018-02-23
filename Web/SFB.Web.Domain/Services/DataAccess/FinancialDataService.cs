using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using SFB.Web.Common;
using SFB.Web.DAL;
using SFB.Web.DAL.Repositories;
using SFB.Web.Domain.Models;

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

        public List<string> GetActiveTermsByDataGroup(string dataGroup, string format)
        {
            return _dataCollectionManager.GetActiveTermsByDataGroup(dataGroup, format);
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
