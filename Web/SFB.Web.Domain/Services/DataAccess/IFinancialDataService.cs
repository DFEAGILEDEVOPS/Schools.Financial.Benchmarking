using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using SFB.Web.Common;
using SFB.Web.Domain.Models;

namespace SFB.Web.Domain.Services.DataAccess
{
    public interface IFinancialDataService
    {
        Document GetSchoolDataDocument(string urn, string term, SchoolFinancialType schoolFinancialType, CentralFinancingType cFinance = CentralFinancingType.Exclude);
        dynamic GetAcademiesByMatNumber(string term, string matNo);
        Document GetMATDataDocument(string matNo, string term, MatFinancingType matFinance);
        Task<List<Document>> SearchSchoolsByCriteria(BenchmarkCriteria criteria, EstablishmentType estType);
        Task<int> SearchSchoolsCountByCriteria(BenchmarkCriteria criteria, EstablishmentType estType);
        int GetLatestDataYearPerSchoolType(SchoolFinancialType type);
        List<string> GetActiveTermsByDataGroup(string dataGroup, string format);
        int GetLatestDataYearForTrusts();
    }
}
