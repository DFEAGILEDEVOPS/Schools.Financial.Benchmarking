using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using SFB.Web.Common;
using SFB.Web.Common.DataObjects;

namespace SFB.Web.Domain.Services.DataAccess
{
    public interface IFinancialDataService : ITermYearDataService
    {
        List<AcademiesContextualDataObject> GetAcademiesByMatNumber(string term, string matNo);

        //Document GetMATDataDocument(string matNo, string term, MatFinancingType matFinance);
        SchoolTrustFinancialDataObject GetTrustFinancialDataObject(string matNo, string term, MatFinancingType matFinance);

        //Document GetSchoolDataDocument(int urn, string term, EstablishmentType schoolFinancialType, CentralFinancingType cFinance = CentralFinancingType.Exclude);
        SchoolTrustFinancialDataObject GetSchoolFinancialDataObject(int urn, string term, EstablishmentType schoolFinancialType, CentralFinancingType cFinance = CentralFinancingType.Exclude);

        //Task<IEnumerable<Document>> GetSchoolDataDocumentAsync(int urn, string term, EstablishmentType schoolFinancialType, CentralFinancingType cFinance = CentralFinancingType.Exclude);
        Task<IEnumerable<SchoolTrustFinancialDataObject>> GetSchoolFinancialDataObjectAsync(int urn, string term, EstablishmentType schoolFinancialType, CentralFinancingType cFinance = CentralFinancingType.Exclude);

        //Task<IEnumerable<Document>> GetMATDataDocumentAsync(string matNo, string term, MatFinancingType matFinance);
        Task<IEnumerable<SchoolTrustFinancialDataObject>> GetTrustFinancialDataObjectAsync(string matNo, string term, MatFinancingType matFinance);

        Task<List<Document>> SearchSchoolsByCriteriaAsync(BenchmarkCriteria criteria, EstablishmentType estType);
        Task<int> SearchSchoolsCountByCriteriaAsync(BenchmarkCriteria criteria, EstablishmentType estType);
        Task<List<Document>> SearchTrustsByCriteriaAsync(BenchmarkCriteria criteria);
        Task<int> SearchTrustCountByCriteriaAsync(BenchmarkCriteria criteria);
        Task<int> GetEstablishmentRecordCountAsync(string term, EstablishmentType estType);
    }

    public interface ITermYearDataService
    {
        List<string> GetActiveTermsForMatCentral();
        List<string> GetActiveTermsForAcademies();
        List<string> GetActiveTermsForMaintained();
        int GetLatestFinancialDataYear();        
        int GetLatestDataYearPerEstabType(EstablishmentType type);
    }
}
