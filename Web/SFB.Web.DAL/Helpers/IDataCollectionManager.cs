using System.Collections.Generic;
using SFB.Web.Common;

namespace SFB.Web.DAL.Helpers
{
    public interface IDataCollectionManager
    {
        int GetLatestFinancialDataYear();
        int GetLatestFinancialDataYearForTrusts();
        int GetLatestFinancialDataYearPerSchoolType(SchoolFinancialType schoolType);
        string GetCollectionIdByTermByDataGroup(string term, string dataGroup);
        string GetActiveCollectionByDataGroup(string dataGroup);
        List<string> GetActiveCollectionsByDataGroup(string dataGroup);
        List<string> GetActiveTermsForMatCentral();
        List<string> GetActiveTermsForAcademies();
        string GetLatestActiveTermByDataGroup(string dataGroup);       
    }
}
