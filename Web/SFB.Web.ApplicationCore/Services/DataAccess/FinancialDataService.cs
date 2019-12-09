using System.Collections.Generic;
using System.Threading.Tasks;
using SFB.Web.ApplicationCore.Entities;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Helpers;
using System.Linq;
using SFB.Web.ApplicationCore.DataAccess;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Helpers.Constants;
using System;

namespace SFB.Web.ApplicationCore.Services.DataAccess
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

        public List<SchoolTrustFinancialDataObject> GetMultipleTrustDataObjectsByCompanyNumbers(List<int> companyNos)
        {
            var term = GetActiveTermsForMatCentral().First();            
            var trustList = _financialDataRepository.GetMultipleTrustFinancialDataObjects(companyNos, term, MatFinancingType.TrustOnly);
            return trustList;
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
            return await _financialDataRepository.SearchSchoolsByCriteriaAsync(criteria, estType, false);
        }

        public async Task<List<SchoolTrustFinancialDataObject>> SearchSchoolsByCriteriaAsync(BenchmarkCriteria criteria, EstablishmentType estType, bool excludePartial)
        {
            return await _financialDataRepository.SearchSchoolsByCriteriaAsync(criteria, estType, excludePartial);
        }

        public async Task<List<SchoolTrustFinancialDataObject>> SearchTrustsByCriteriaAsync(BenchmarkCriteria criteria)
        {
            return await _financialDataRepository.SearchTrustsByCriteriaAsync(criteria);
        }

        public async Task<int> SearchSchoolsCountByCriteriaAsync(BenchmarkCriteria criteria, EstablishmentType estType, bool excludePartial = false)
        {
            return await _financialDataRepository.SearchSchoolsCountByCriteriaAsync(criteria, estType, excludePartial);
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

        public async Task<List<FinancialDataModel>> GetFinancialDataForSchoolsAsync(List<SchoolSearchModel> schools, CentralFinancingType centralFinancing = CentralFinancingType.Include)
        {
            var models = new List<FinancialDataModel>();

            var taskList = new List<Task<IEnumerable<SchoolTrustFinancialDataObject>>>();
            foreach (var school in schools)
            {
                var estabType = (EstablishmentType)Enum.Parse(typeof(EstablishmentType), school.EstabType);
                var latestYear = this.GetLatestDataYearPerEstabType(estabType);
                var term = SchoolFormatHelpers.FinancialTermFormatAcademies(latestYear);

                var task = this.GetSchoolFinancialDataObjectAsync(Int32.Parse(school.Urn), term, estabType, centralFinancing);
                taskList.Add(task);
            }

            for (var i = 0; i < schools.Count; i++)
            {
                var estabType = (EstablishmentType)Enum.Parse(typeof(EstablishmentType), schools[i].EstabType);
                var latestYear = this.GetLatestDataYearPerEstabType(estabType);
                var term = SchoolFormatHelpers.FinancialTermFormatAcademies(latestYear);
                var taskResult = await taskList[i];
                var resultDocument = taskResult?.FirstOrDefault();

                if (estabType == EstablishmentType.Academies && centralFinancing == CentralFinancingType.Include && resultDocument == null)//if nothing found in -Allocs collection try to source it from (non-allocated) Academies data
                {
                    resultDocument = (await this.GetSchoolFinancialDataObjectAsync(Int32.Parse(schools[i].Urn), term, estabType, CentralFinancingType.Exclude))
                        ?.FirstOrDefault();
                }

                if (resultDocument != null && resultDocument.DidNotSubmit)//School did not submit finance, return & display "no data" in the charts
                {
                    resultDocument = null;
                }

                models.Add(new FinancialDataModel(schools[i].Urn, term, resultDocument, (EstablishmentType)Enum.Parse(typeof(EstablishmentType), schools[i].EstabType)));
            }

            return models;
        }
    }
}
