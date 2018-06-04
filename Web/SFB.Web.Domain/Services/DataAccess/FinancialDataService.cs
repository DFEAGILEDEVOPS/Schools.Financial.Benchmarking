﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using SFB.Web.Common;
using SFB.Web.DAL.Helpers;
using SFB.Web.DAL.Repositories;
using SFB.Web.DAL;

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
        
        public async Task<IEnumerable<Document>> GetSchoolDataDocumentAsync(string urn, string term, EstablishmentType schoolFinancialType, CentralFinancingType cFinance)
        {
            return await _financialDataRepository.GetSchoolDataDocumentAsync(urn, term, schoolFinancialType, cFinance);
        }

        public Document GetSchoolDataDocument(int urn, string term, EstablishmentType schoolFinancialType, CentralFinancingType cFinance)
        {
            return _financialDataRepository.GetSchoolDataDocument(urn, term, schoolFinancialType, cFinance);
        }

        public Document GetMATDataDocument(string matNo, string term, MatFinancingType matFinance)
        {
            return _financialDataRepository.GetMATDataDocument(matNo, term, matFinance);
        }

        public async Task<IEnumerable<Document>> GetMATDataDocumentAsync(string matNo, string term, MatFinancingType matFinance)
        {
            return await _financialDataRepository.GetMATDataDocumentAsync(matNo, term, matFinance);
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

        public async Task<List<Document>> SearchSchoolsByCriteriaAsync(BenchmarkCriteria criteria, EstablishmentType estType)
        {
            return await _financialDataRepository.SearchSchoolsByCriteriaAsync(criteria, estType);
        }

        public async Task<List<Document>> SearchTrustsByCriteriaAsync(BenchmarkCriteria criteria)
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

        public dynamic GetAcademiesByMatNumber(string term, string matNo)
        {
            return _financialDataRepository.GetAcademiesByMatNumber(term, matNo);
        }
    }
}
