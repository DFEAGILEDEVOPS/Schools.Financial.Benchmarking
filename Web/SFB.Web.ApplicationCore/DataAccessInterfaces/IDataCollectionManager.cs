using System.Collections.Generic;
using System.Threading.Tasks;
using SFB.Web.ApplicationCore.Helpers.Enums;

namespace SFB.Web.ApplicationCore.DataAccess
{
    public interface IDataCollectionManager
    {
        Task<int> GetOverallLatestFinancialDataYearAsync();
        Task<int> GetLatestFinancialDataYearPerEstabTypeAsync(EstablishmentType estabType);
        Task<string> GetCollectionIdByTermByDataGroupAsync(string term, string dataGroup);
        Task<List<string>> GetActiveCollectionsByDataGroupAsync(string dataGroup);
        Task<string> GetLatestActiveCollectionByDataGroupAsync(string dataGroup);
        Task<List<string>> GetActiveTermsByDataGroupAsync(string dataGroup);
        Task<string> GetLatestActiveCollectionIdByDataGroupAsync(string dataGroup);
    }
}
