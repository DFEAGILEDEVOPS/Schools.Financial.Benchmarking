using System.Collections.Generic;
using System.Threading.Tasks;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Entities;

namespace SFB.Web.ApplicationCore.DataAccess
{
    public interface IFinancialDataRepository
    {
        Task<List<AcademiesContextualDataObject>> GetAcademiesContextualDataObjectAsync(string term, int companyNo);
        Task<SchoolTrustFinancialDataObject> GetTrustFinancialDataObjectAsync(int companyNo, string term, MatFinancingType matFinance);
        Task<List<SchoolTrustFinancialDataObject>> GetMultipleTrustFinancialDataObjectsAsync(List<int> companyNoList, string term, MatFinancingType matFinance);
        Task<SchoolTrustFinancialDataObject> GetTrustFinancialDataObjectByMatNameAsync(string matName, string term, MatFinancingType matFinance);   
        Task<SchoolTrustFinancialDataObject> GetSchoolFinanceDataObjectAsync(int urn, string term, EstablishmentType schoolFinancialType, CentralFinancingType cFinance = CentralFinancingType.Exclude);
        Task<SchoolTrustFinancialDataObject> GetSchoolFinancialDataObjectAsync(int urn, string term, EstablishmentType schoolFinancialType, CentralFinancingType cFinance = CentralFinancingType.Exclude);
        Task<int> SearchTrustCountByCriteriaAsync(BenchmarkCriteria criteria);
        Task<List<SchoolTrustFinancialDataObject>> SearchTrustsByCriteriaAsync(BenchmarkCriteria criteria);
        Task<int> SearchSchoolsCountByCriteriaAsync(BenchmarkCriteria criteria, EstablishmentType estType, bool excludePartial = false);
        Task<List<SchoolTrustFinancialDataObject>> SearchSchoolsByCriteriaAsync(BenchmarkCriteria criteria, EstablishmentType estType, bool excludePartial = false);
        Task<int> GetEstablishmentRecordCountAsync(string term, EstablishmentType estType);
    }
}
