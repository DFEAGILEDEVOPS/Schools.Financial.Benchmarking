using System.Collections.Generic;
using SFB.Web.Common;

namespace SFB.Web.Domain.DataAccessInterfaces
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
