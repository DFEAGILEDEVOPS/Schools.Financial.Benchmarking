using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json.Linq;
using SFB.Web.Common;

namespace SFB.Web.DAL.Helpers
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
            var colls = GetActiveCollections();
            var result = colls.Where(w => w.GetProperty("data-group", "") == dataGroup)
                .Select(s => s.GetProperty("name", ""))
                .ToList();
            return result;
        }

        public List<string> GetActiveTermsForMatCentral()
        {
            return GetActiveTermsByDataGroup(DataGroups.MATCentral, "{0} / {1}");
        }

        public List<string> GetActiveTermsForAcademies()
        {
            return GetActiveTermsByDataGroup(DataGroups.Academies, "{0} / {1}");
        }

        public string GetActiveCollectionByDataGroup(string dataGroup)
        {
            return
                GetActiveCollectionsByDataGroup(dataGroup).OrderByDescending(o => o.Split('-').First()).FirstOrDefault();
        }

        public int GetLatestFinancialDataYear()
        {
            var maintainedLatestCollectionId = GetLatestActiveTermByDataGroup("Maintained");
            var maintanedLatestDataYear =
                Int32.Parse(
                    maintainedLatestCollectionId.Substring(
                        maintainedLatestCollectionId.LastIndexOf("-", StringComparison.Ordinal) + 1));
            var academiesLatestCollectionId = GetLatestActiveTermByDataGroup("Academies");
            var academiesLatestDataYear =
                Int32.Parse(
                    academiesLatestCollectionId.Substring(
                        academiesLatestCollectionId.LastIndexOf("-", StringComparison.Ordinal) + 1));

            return maintanedLatestDataYear > academiesLatestDataYear ? maintanedLatestDataYear : academiesLatestDataYear;

        }

        public int GetLatestFinancialDataYearPerSchoolType(SchoolFinancialType type)
        {
            var latestCollectionId = GetLatestActiveTermByDataGroup(type.ToString());
            return int.Parse(latestCollectionId.Split('-').Last());
        }

        public int GetLatestFinancialDataYearForTrusts()
        {
            var latestCollectionId = GetLatestActiveTermByDataGroup(DataGroups.MATCentral);
            return int.Parse(latestCollectionId.Split('-').Last());
        }

        public string GetLatestActiveTermByDataGroup(string dataGroup)
        {
            var latestTerm = GetActiveCollections()
                .Where(w => w.GetProperty("data-group", "") == dataGroup)
                .OrderByDescending(o => o.GetProperty("term", ""))
                .FirstOrDefault();
            return latestTerm?.GetProperty("name", "") ?? string.Empty;
        }

        public string GetCollectionIdByTermByDataGroup(string term, string dataGroup)
        {
            if (dataGroup == DataGroups.MATDistributed)
            {
                return GetActiveCollectionsByDataGroup(dataGroup)
                    .SingleOrDefault(sod => sod.Split('-')[3] == term.Split(' ').Last());
            }
            return GetActiveCollectionsByDataGroup(dataGroup)
                .SingleOrDefault(sod => sod.Split('-').Last() == term.Split(' ').Last());
        }

        private List<JObject> GetActiveCollections()
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

        private List<string> GetActiveTermsByDataGroup(string dataGroup, string format = "{0} - {1}")
        {
            var colls = GetActiveCollections();
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
    }
}