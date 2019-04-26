using System.Collections.Generic;
using System.Threading.Tasks;
using SFB.Web.Common;
using SFB.Web.Common.DataObjects;

namespace SFB.Web.DAL.Repositories
{
    public interface IFinancialDataRepository
    {
        List<AcademiesContextualDataObject> GetAcademiesContextualDataObject(string term, int companyNo);
        SchoolTrustFinancialDataObject GetTrustFinancialDataObject(int companyNo, string term, MatFinancingType matFinance);
        SchoolTrustFinancialDataObject GetTrustFinancialDataObjectByMatNo(string matNo, string term, MatFinancingType matFinance);
        SchoolTrustFinancialDataObject GetTrustFinancialDataObjectByMatName(string matName, string term, MatFinancingType matFinance);
        Task<IEnumerable<SchoolTrustFinancialDataObject>> GetTrustFinancialDataObjectAsync(int companyNo, string term, MatFinancingType matFinance);        
        Task<IEnumerable<SchoolTrustFinancialDataObject>> GetSchoolFinanceDataObjectAsync(int urn, string term, EstablishmentType schoolFinancialType, CentralFinancingType cFinance = CentralFinancingType.Exclude);
        SchoolTrustFinancialDataObject GetSchoolFinancialDataObject(int urn, string term, EstablishmentType schoolFinancialType, CentralFinancingType cFinance = CentralFinancingType.Exclude);
        Task<int> SearchTrustCountByCriteriaAsync(BenchmarkCriteria criteria);
        Task<List<SchoolTrustFinancialDataObject>> SearchTrustsByCriteriaAsync(BenchmarkCriteria criteria);
        Task<int> SearchSchoolsCountByCriteriaAsync(BenchmarkCriteria criteria, EstablishmentType estType);
        Task<List<SchoolTrustFinancialDataObject>> SearchSchoolsByCriteriaAsync(BenchmarkCriteria criteria, EstablishmentType estType);
        Task<int> GetEstablishmentRecordCountAsync(string term, EstablishmentType estType);
    }
}
