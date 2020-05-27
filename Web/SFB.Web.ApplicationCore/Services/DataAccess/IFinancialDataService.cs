using System.Collections.Generic;
using System.Threading.Tasks;
using SFB.Web.ApplicationCore.Entities;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Helpers.Enums;

namespace SFB.Web.ApplicationCore.Services.DataAccess
{
    public interface IFinancialDataService : ITermYearDataService
    {
        Task<List<AcademiesContextualDataObject>> GetAcademiesByCompanyNumberAsync(string term, int companyNo);
        Task<List<SchoolTrustFinancialDataObject>> GetMultipleTrustDataObjectsByCompanyNumbersAsync(List<int> companyNos);
        Task<SchoolTrustFinancialDataObject> GetTrustFinancialDataObjectByMatNameAsync(string matName, string term, MatFinancingType matFinance);
        Task<FinancialDataModel> GetSchoolsLatestFinancialDataModelAsync(int urn, EstablishmentType schoolFinancialType);
        Task<SchoolTrustFinancialDataObject> GetSchoolFinancialDataObjectAsync(int urn, EstablishmentType schoolFinancialType, CentralFinancingType cFinance = CentralFinancingType.Exclude);
        Task<SchoolTrustFinancialDataObject> GetSchoolFinancialDataObjectAsync(int urn, string term, EstablishmentType schoolFinancialType, CentralFinancingType cFinance = CentralFinancingType.Exclude);
        Task<SchoolTrustFinancialDataObject> GetTrustFinancialDataObjectAsync(int companyNo, string term, MatFinancingType matFinance);
        Task<List<SchoolTrustFinancialDataObject>> SearchTrustsByCriteriaAsync(BenchmarkCriteria criteria);
        Task<List<SchoolTrustFinancialDataObject>> SearchSchoolsByCriteriaAsync(BenchmarkCriteria criteria, EstablishmentType estType);
        Task<List<SchoolTrustFinancialDataObject>> SearchSchoolsByCriteriaAsync(BenchmarkCriteria criteria, EstablishmentType estType, bool excludePartial);
        Task<int> SearchSchoolsCountByCriteriaAsync(BenchmarkCriteria criteria, EstablishmentType estType, bool excludePartial = false);
        Task<int> SearchTrustCountByCriteriaAsync(BenchmarkCriteria criteria);
        Task<int> GetEstablishmentRecordCountAsync(string term, EstablishmentType estType);
        Task<List<FinancialDataModel>> GetFinancialDataForSchoolsAsync(List<SchoolSearchModel> schools, CentralFinancingType centralFinancing = CentralFinancingType.Include);
    }

    public interface ITermYearDataService
    {
        Task<List<string>> GetActiveTermsForMatCentralAsync();
        Task<List<string>> GetActiveTermsForAcademiesAsync();
        Task<List<string>> GetActiveTermsForMaintainedAsync();
        Task<int> GetLatestFinancialDataYearAsync();        
        Task<int> GetLatestDataYearPerEstabTypeAsync(EstablishmentType type);
    }
}
