using System.Collections.Generic;
using SFB.Web.Common;

namespace SFB.Web.DAL.Helpers
{
    public interface IDataCollectionManager
    {
        int GetLatestFinancialDataYear();
        int GetLatestFinancialDataYearPerEstabType(EstabType estabType);
        string GetCollectionIdByTermByDataGroup(string term, string dataGroup);
        string GetActiveCollectionByDataGroup(string dataGroup);
        List<string> GetActiveCollectionsByDataGroup(string dataGroup);
        List<string> GetActiveTermsForMatCentral();
        List<string> GetActiveTermsForAcademies();
        List<string> GetActiveTermsForMaintained();
        string GetLatestActiveTermByDataGroup(string dataGroup);
    }
}
