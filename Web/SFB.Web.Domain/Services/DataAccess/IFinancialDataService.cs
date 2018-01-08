using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using SFB.Web.Domain.Helpers.Enums;
using SFB.Web.Domain.Models;

namespace SFB.Web.Domain.Services.DataAccess
{
    public interface IFinancialDataService
    {
        int GetLatestDataYearPerSchoolType(SchoolFinancialType schoolType);
        int GetLatestDataYearForTrusts();
        Document GetSchoolDataDocument(string urn, string term, SchoolFinancialType schoolFinancialType, CentralFinancingType cFinance = CentralFinancingType.Exclude);
        dynamic GetAcademiesByMatNumber(string term, string matNo);
        Document GetMATDataDocument(string matNo, string term, MatFinancingType matFinance);
        Task<List<Document>> SearchSchoolsByCriteria(BenchmarkCriteria criteria, EstablishmentType estType);
        Task<int> SearchSchoolsCountByCriteria(BenchmarkCriteria criteria, EstablishmentType estType);
        List<string> GetActiveTermsByDataGroup(string dataGroup, string format = "{0} - {1}");
        string GetActiveCollectionByDataGroup(string dataGroup);
    }
}
