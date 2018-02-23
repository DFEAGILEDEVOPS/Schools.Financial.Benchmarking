using System.Collections.Generic;
using SFB.Web.Common;

namespace SFB.Web.DAL
{
    public interface IDataCollectionManager
    {
        int GetLatestFinancialDataYear();
        int GetLatestFinancialDataYearForTrusts();
        int GetLatestFinancialDataYearPerSchoolType(SchoolFinancialType schoolType);

        string GetCollectionIdByTermByDataGroup(string term, string dataGroup);
        string GetActiveCollectionByDataGroup(string dataGroup);
        List<string> GetActiveCollectionsByDataGroup(string dataGroup);
        List<string> GetActiveTermsByDataGroup(string dataGroup, string format);
        string GetLatestActiveTermByDataGroup(string dataGroup);       
    }
}
