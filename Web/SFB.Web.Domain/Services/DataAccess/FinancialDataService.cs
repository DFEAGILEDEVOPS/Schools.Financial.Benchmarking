using System.Collections.Generic;
using System.Threading.Tasks;
using SFB.Web.Common;
using SFB.Web.DAL.Helpers;
using SFB.Web.DAL.Repositories;
using SFB.Web.DAL;
using SFB.Web.Common.DataObjects;
using SFB.Web.Domain.Models;
using SFB.Web.Domain.Helpers;

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

        public async Task<IEnumerable<SchoolTrustFinancialDataObject>> GetSchoolFinancialDataObjectAsync(int urn, string term, EstablishmentType schoolFinancialType, CentralFinancingType cFinance)
        {
            return await _financialDataRepository.GetSchoolFinanceDataObjectAsync(urn, term, schoolFinancialType, cFinance);
        }

        public SchoolTrustFinancialDataObject GetSchoolFinancialDataObject(int urn, string term, EstablishmentType schoolFinancialType, CentralFinancingType cFinance = CentralFinancingType.Exclude)
        {
            return _financialDataRepository.GetSchoolFinancialDataObject(urn, term, schoolFinancialType, cFinance);
        }

        public FinancialDataModel GetSchoolsLatestFinancialDataModel(int urn, EstablishmentType schoolFinancialType)
        {
            var latestYear = GetLatestDataYearPerEstabType(schoolFinancialType);
            var term = SchoolFormatHelpers.FinancialTermFormatAcademies(latestYear);
            var schoolFinancialDataObject = _financialDataRepository.GetSchoolFinancialDataObject(urn, term, schoolFinancialType);
            return new FinancialDataModel(urn.ToString(), term, schoolFinancialDataObject, schoolFinancialType);
        }

        public SchoolTrustFinancialDataObject GetTrustFinancialDataObject(int companyNo, string term, MatFinancingType matFinance)
        {
            return _financialDataRepository.GetTrustFinancialDataObject(companyNo, term, matFinance);
        }

        public SchoolTrustFinancialDataObject GetTrustFinancialDataObjectByMatNo(string matNo, string term, MatFinancingType matFinance)
        {
            return _financialDataRepository.GetTrustFinancialDataObjectByMatNo(matNo, term, matFinance);
        }

        public SchoolTrustFinancialDataObject GetTrustFinancialDataObjectByMatName(string matName, string term, MatFinancingType matFinance)
        {
            return _financialDataRepository.GetTrustFinancialDataObjectByMatName(matName, term, matFinance);
        }

        public async Task<IEnumerable<SchoolTrustFinancialDataObject>> GetTrustFinancialDataObjectAsync(int companyNo, string term, MatFinancingType matFinance)
        {
            return await _financialDataRepository.GetTrustFinancialDataObjectAsync(companyNo, term, matFinance);
        }

        public int GetLatestFinancialDataYear()
        {
            return _dataCollectionManager.GetOverallLatestFinancialDataYear();
        }

        public int GetLatestDataYearPerEstabType(EstablishmentType type)
        {
            return _dataCollectionManager.GetLatestFinancialDataYearPerEstabType(type);
        }

        public List<string> GetActiveTermsForMatCentral()
        {
            return _dataCollectionManager.GetActiveTermsByDataGroup(DataGroups.MATCentral);
        }

        public List<string> GetActiveTermsForMaintained()
        {
            return _dataCollectionManager.GetActiveTermsByDataGroup(DataGroups.Maintained);
        }

        public List<string> GetActiveTermsForAcademies()
        {
            return _dataCollectionManager.GetActiveTermsByDataGroup(DataGroups.Academies);
        }

        public async Task<List<SchoolTrustFinancialDataObject>> SearchSchoolsByCriteriaAsync(BenchmarkCriteria criteria, EstablishmentType estType)
        {
            return await _financialDataRepository.SearchSchoolsByCriteriaAsync(criteria, estType);
        }

        public async Task<List<SchoolTrustFinancialDataObject>> SearchTrustsByCriteriaAsync(BenchmarkCriteria criteria)
        {
            return await _financialDataRepository.SearchTrustsByCriteriaAsync(criteria);
        }

        public async Task<int> SearchSchoolsCountByCriteriaAsync(BenchmarkCriteria criteria, EstablishmentType estType)
        {
            return await _financialDataRepository.SearchSchoolsCountByCriteriaAsync(criteria, estType);
        }

        public async Task<int> SearchTrustCountByCriteriaAsync(BenchmarkCriteria criteria)
        {
            return await _financialDataRepository.SearchTrustCountByCriteriaAsync(criteria);
        }

        public async Task<int> GetEstablishmentRecordCountAsync(string term, EstablishmentType estType)
        {
            return await _financialDataRepository.GetEstablishmentRecordCountAsync(term, estType);
        }

        public List<AcademiesContextualDataObject> GetAcademiesByCompanyNumber(string term, int companyNo)
        {
            return _financialDataRepository.GetAcademiesContextualDataObject(term, companyNo);
        }
    }
}
