using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json.Linq;
using SFB.Web.ApplicationCore.DataAccess;
using SFB.Web.ApplicationCore.Helpers.Constants;
using SFB.Web.ApplicationCore.Helpers.Enums;

namespace SFB.Web.Infrastructure.Helpers
{
    public class DataCollectionManager : IDataCollectionManager
    {
        private static readonly string DatabaseId = ConfigurationManager.AppSettings["database"];
        private static DocumentClient _client;

        public DataCollectionManager()
        {
            _client = new DocumentClient(new Uri(ConfigurationManager.AppSettings["endpoint"]),
                ConfigurationManager.AppSettings["authKey"],
                new ConnectionPolicy
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp
                });
        }

        public List<string> GetActiveCollectionsByDataGroup(string dataGroup)
        {
            var colls = GetCachedActiveCollections();
            var result = colls.Where(w => w.GetProperty("data-group", "") == dataGroup)
                .Select(s => s.GetProperty("name", ""))
                .ToList();
            return result;
        }

        public List<string> GetActiveTermsByDataGroup(string dataGroup)
        {
            string format = dataGroup == DataGroups.Maintained ? "{0} - {1}" : "{0} / {1}";

            var colls = GetCachedActiveCollections();
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

        public string GetLatestActiveCollectionByDataGroup(string dataGroup)
        {
            return
                GetActiveCollectionsByDataGroup(dataGroup).OrderByDescending(o => o.Split('-').First()).FirstOrDefault();
        }

        public int GetOverallLatestFinancialDataYear()
        {
            var maintainedLatestCollectionId = GetLatestActiveCollectionIdByDataGroup("Maintained");
            var maintanedLatestDataYear =
                Int32.Parse(
                    maintainedLatestCollectionId.Substring(
                        maintainedLatestCollectionId.LastIndexOf("-", StringComparison.Ordinal) + 1));
            var academiesLatestCollectionId = GetLatestActiveCollectionIdByDataGroup("Academies");
            var academiesLatestDataYear =
                Int32.Parse(
                    academiesLatestCollectionId.Substring(
                        academiesLatestCollectionId.LastIndexOf("-", StringComparison.Ordinal) + 1));

            return maintanedLatestDataYear > academiesLatestDataYear ? maintanedLatestDataYear : academiesLatestDataYear;

        }

        public int GetLatestFinancialDataYearPerEstabType(EstablishmentType type)
        {
            var latestCollectionId = GetLatestActiveCollectionIdByDataGroup(type.ToDataGroup());
            return int.Parse(latestCollectionId.Split('-').Last());
        }

        public string GetLatestActiveCollectionIdByDataGroup(string dataGroup)
        {
            var latestTerm = GetCachedActiveCollections()
                .Where(w => w.GetProperty("data-group", "") == dataGroup)
                .OrderByDescending(o => o.GetProperty("term", ""))
                .FirstOrDefault();
            return latestTerm?.GetProperty("name", "") ?? string.Empty;
        }

        public string GetCollectionIdByTermByDataGroup(string term, string dataGroup)
        {
            return GetActiveCollectionsByDataGroup(dataGroup)
                .SingleOrDefault(sod => sod.Split('-').Last() == term.Split(' ').Last());
        }

        private List<JObject> GetCachedActiveCollections()
        {
            var docs = (List<JObject>)HttpContext.Current.Cache.Get("SFBActiveCollectionList");

            if (docs == null)
            {
                var query = _client.CreateDocumentQuery(UriFactory.CreateDocumentCollectionUri(DatabaseId, "fibre-directory"), "SELECT * FROM c WHERE c['active'] = 'Y'").ToList();

                docs = query.Select(d => JObject.Parse(d.ToString())).Cast<JObject>().ToList();

                HttpContext.Current.Cache.Insert("SFBActiveCollectionList", docs, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(60));
            }

            return docs;
        }
    }
}