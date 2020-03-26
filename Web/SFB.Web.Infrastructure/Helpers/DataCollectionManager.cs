using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Newtonsoft.Json.Linq;
using SFB.Web.ApplicationCore.DataAccess;
using SFB.Web.ApplicationCore.Helpers.Constants;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Services;

namespace SFB.Web.Infrastructure.Helpers
{
    public class DataCollectionManager : IDataCollectionManager
    {
        private readonly string _databaseId;
        private static CosmosClient _client;
        private readonly Container _container;
        private readonly IActiveCollectionsService _activeCollectionsService;

        public DataCollectionManager(IActiveCollectionsService activeCollectionsService)
        {
            _activeCollectionsService = activeCollectionsService;

            var clientBuilder = new CosmosClientBuilder(ConfigurationManager.AppSettings["endpoint"],
                ConfigurationManager.AppSettings["authKey"]);

            _client = clientBuilder
                                .WithConnectionModeDirect()
                                .Build();

            _databaseId = ConfigurationManager.AppSettings["database"];

            _container = _client.GetContainer(_databaseId, "fibre-directory");
        }

        public DataCollectionManager(CosmosClient cosmosClient, string databaseId, IActiveCollectionsService activeCollectionsService)
        {          
            _client = cosmosClient;

            _databaseId = databaseId;

            _container = _client.GetContainer(_databaseId, "fibre-directory");

            _activeCollectionsService = activeCollectionsService;
        }

        public async Task<List<string>> GetActiveCollectionsByDataGroupAsync(string dataGroup)
        {
            var colls = await GetCachedActiveCollectionsAsync();
            var result = colls.Where(w => w.GetProperty("data-group", "") == dataGroup)
                .Select(s => s.GetProperty("name", ""))
                .ToList();
            return result;
        }

        public async Task<List<string>> GetActiveTermsByDataGroupAsync(string dataGroup)
        {
            string format = dataGroup == DataGroups.Maintained ? "{0} - {1}" : "{0} / {1}";

            var colls = await GetCachedActiveCollectionsAsync();
            return
                colls.Where(w => w.GetProperty("data-group", "") == dataGroup)
                    .OrderByDescending(o => o.GetProperty("term", 0))
                    .Select(s =>
                    {
                        var term = s.GetProperty("term", 0);
                        return string.Format(format, term - 1, term);
                    })
                    .ToList();
        }

        public async Task<string> GetLatestActiveCollectionByDataGroupAsync(string dataGroup)
        {          
            var activeCollections = await GetActiveCollectionsByDataGroupAsync(dataGroup);

            return activeCollections.OrderByDescending(o => o.Split('-').First()).FirstOrDefault();
        }

        public async Task<int> GetOverallLatestFinancialDataYearAsync()
        {
            string maintainedLatestCollectionId = await GetLatestActiveCollectionIdByDataGroupAsync("Maintained");
            var maintanedLatestDataYear =
                Int32.Parse(
                    maintainedLatestCollectionId.Substring(
                        maintainedLatestCollectionId.LastIndexOf("-", StringComparison.Ordinal) + 1));
            var academiesLatestCollectionId = await GetLatestActiveCollectionIdByDataGroupAsync("Academies");
            var academiesLatestDataYear =
                Int32.Parse(
                    academiesLatestCollectionId.Substring(
                        academiesLatestCollectionId.LastIndexOf("-", StringComparison.Ordinal) + 1));

            return maintanedLatestDataYear > academiesLatestDataYear ? maintanedLatestDataYear : academiesLatestDataYear;

        }

        public async Task<int> GetLatestFinancialDataYearPerEstabTypeAsync(EstablishmentType type)
        {
            var latestCollectionId = await GetLatestActiveCollectionIdByDataGroupAsync(type.ToDataGroup());
            return int.Parse(latestCollectionId.Split('-').Last());
        }

        public async Task<string> GetLatestActiveCollectionIdByDataGroupAsync(string dataGroup)
        {
            var latestTerm = (await GetCachedActiveCollectionsAsync())
                .Where(w => w.GetProperty("data-group", "") == dataGroup)
                .OrderByDescending(o => o.GetProperty("term", ""))
                .FirstOrDefault();
            return latestTerm?.GetProperty("name", "") ?? string.Empty;
        }

        public async Task<string> GetCollectionIdByTermByDataGroupAsync(string term, string dataGroup)
        {
            return (await GetActiveCollectionsByDataGroupAsync(dataGroup))
                .SingleOrDefault(sod => sod.Split('-').Last() == term.Split(' ').Last());
        }

        private async Task<List<JObject>> GetCachedActiveCollectionsAsync()
        {         
            var docs = _activeCollectionsService.GetActiveCollectionsList();

            if (docs == null)
            {
                var queryString = $"SELECT * FROM c WHERE c['active'] = 'Y'";

                var query = _container.GetItemQueryIterator<JObject>(new QueryDefinition(queryString));

                var results = new List<JObject>();
                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();

                    results.AddRange(response.ToList());
                }

                docs = results.Select(d => JObject.Parse(d.ToString())).Cast<JObject>().ToList();
                
                _activeCollectionsService.SetActiveCollectionsList(docs);
            }

            return docs;
        }
    }
}