using System.Collections.Generic;
using SFB.Web.ApplicationCore.Helpers.Enums;

namespace SFB.Web.ApplicationCore.DataAccess
{
    public interface IDataCollectionManager
    {
        int GetOverallLatestFinancialDataYear();
        int GetLatestFinancialDataYearPerEstabType(EstablishmentType estabType);
        string GetCollectionIdByTermByDataGroup(string term, string dataGroup);
        List<string> GetActiveCollectionsByDataGroup(string dataGroup);
        string GetLatestActiveCollectionByDataGroup(string dataGroup);
        List<string> GetActiveTermsByDataGroup(string dataGroup);
        string GetLatestActiveCollectionIdByDataGroup(string dataGroup);
    }
}
