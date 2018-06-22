using System.Collections.Generic;
using System.Threading.Tasks;
using SFB.Web.Common;
using SFB.Web.Common.DataObjects;

namespace SFB.Web.DAL.Repositories
{
    public interface IFinancialDataRepository
    {
        List<AcademiesContextualDataObject> GetAcademiesContextualDataObject(string term, string matNo);
        SchoolTrustFinancialDataObject GetTrustFinancialDataObject(string matNo, string term, MatFinancingType matFinance);        
        Task<IEnumerable<SchoolTrustFinancialDataObject>> GetTrustFinancialDataObjectAsync(string matNo, string term, MatFinancingType matFinance);        
        Task<IEnumerable<SchoolTrustFinancialDataObject>> GetSchoolFinanceDataObjectAsync(int urn, string term, EstablishmentType schoolFinancialType, CentralFinancingType cFinance = CentralFinancingType.Exclude);
        SchoolTrustFinancialDataObject GetSchoolFinancialDataObject(int urn, string term, EstablishmentType schoolFinancialType, CentralFinancingType cFinance = CentralFinancingType.Exclude);
        Task<int> SearchTrustCountByCriteriaAsync(BenchmarkCriteria criteria);
        Task<List<SchoolTrustFinancialDataObject>> SearchTrustsByCriteriaAsync(BenchmarkCriteria criteria);
        Task<int> SearchSchoolsCountByCriteriaAsync(BenchmarkCriteria criteria, EstablishmentType estType);
        Task<List<SchoolTrustFinancialDataObject>> SearchSchoolsByCriteriaAsync(BenchmarkCriteria criteria, EstablishmentType estType);
        Task<int> GetEstablishmentRecordCountAsync(string term, EstablishmentType estType);
    }
}
