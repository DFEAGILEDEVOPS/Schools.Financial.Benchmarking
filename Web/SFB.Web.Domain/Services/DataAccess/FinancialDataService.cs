using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json.Linq;
using SFB.Web.Domain.Helpers;
using SFB.Web.Domain.Helpers.Constants;
using SFB.Web.Domain.Helpers.Enums;
using SFB.Web.Domain.Models;

namespace SFB.Web.Domain.Services.DataAccess
{
    public class FinancialDataService : IFinancialDataService
    {
        private static readonly string DatabaseId = ConfigurationManager.AppSettings["database"];
        private static DocumentClient _client;

        public FinancialDataService()
        {
            _client = new DocumentClient(new Uri(ConfigurationManager.AppSettings["endpoint"]),
                ConfigurationManager.AppSettings["authKey"],
                new ConnectionPolicy
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp
                });
        }
        
        public Document GetSchoolDataDocument(string urn, string term, SchoolFinancialType schoolFinancialType, CentralFinancingType cFinance)
        {
            var dataGroup = schoolFinancialType.ToString();
            if (schoolFinancialType == SchoolFinancialType.Academies)
            {
                dataGroup = (cFinance == CentralFinancingType.Include) ? DataGroups.MATDistributed : DataGroups.Academies;
            }

            var collectionName = GetCollectionIdByTermByDataGroup(term, dataGroup);

            try
            {
                var query =
                    _client.CreateDocumentQuery<Document>(
                        UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionName),
                        $"SELECT * FROM c WHERE c.URN={urn}");

                var result = query.ToList().FirstOrDefault();

                if (dataGroup == DataGroups.MATDistributed && result == null)//if nothing found in -Distributed collection try to source it from Academies data
                {
                    return GetSchoolDataDocument(urn, term, schoolFinancialType, CentralFinancingType.Exclude);
                }

                if (result != null && result.GetPropertyValue<bool>("DNS"))//School did not submit finance, return & display none in the charts
                {
                    return null;
                }

                return result;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public Document GetMATDataDocument(string matNo, string term, MatFinancingType matFinance)
        {
            var collectionName = GetCollectionIdByTermByDataGroup(term, matFinance == MatFinancingType.TrustOnly ? DataGroups.MATCentral : DataGroups.MATOverview);
            if (collectionName == null)
            {
                return null;
            }

            var res =
                _client.CreateDocumentQuery<Document>(
                    UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionName),
                    $"SELECT * FROM c WHERE c['MATNumber']='{matNo}'"); 

            try
            {
                var result = res.ToList().FirstOrDefault();

                if (result != null && result.GetPropertyValue<bool>("DNS"))
                {
                    var emptyDoc = new Document();
                    emptyDoc.SetPropertyValue("DNS", true);
                    return emptyDoc;
                }
                return result;
            }
            catch (Exception)
            {
                return new Document();
            }

        }

        public int GetLatestDataYear()
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

        public int GetLatestDataYearPerSchoolType(SchoolFinancialType type)
        {
            var latestCollectionId = GetLatestActiveTermByDataGroup(type.ToString());
            return int.Parse(latestCollectionId.Split('-').Last());
        }

        public int GetLatestDataYearForTrusts()
        {
            var latestCollectionId = GetLatestActiveTermByDataGroup(DataGroups.MATCentral);
            return int.Parse(latestCollectionId.Split('-').Last());
        }

        public async Task<List<Document>> SearchSchoolsByCriteria(BenchmarkCriteria criteria, EstablishmentType estType)
        {
            if (estType == EstablishmentType.All)
            {
                var maintainedSchoolsTask = QueryDBCollectionAsync(criteria, "Maintained");
                var academiesTask = QueryDBCollectionAsync(criteria, "Academies");
                var maintainedSchools = (await maintainedSchoolsTask).ToList();
                var academies = (await academiesTask).ToList();
                maintainedSchools.AddRange(academies);
                return maintainedSchools;
            }
            else
            {
                var type = estType == EstablishmentType.Academy ? "Academies" : "Maintained";
                return (await QueryDBCollectionAsync(criteria, type)).ToList();
            }
        }

        public async Task<int> SearchSchoolsCountByCriteria(BenchmarkCriteria criteria, EstablishmentType estType)
        {
            if (estType == EstablishmentType.All)
            {
                var maintainedSchoolsCountTask = QueryDBCollectionForCountAsync(criteria, "Maintained");
                var academiesCountTask = QueryDBCollectionForCountAsync(criteria, "Academies");
                return (await maintainedSchoolsCountTask).First() + (await academiesCountTask).First();
            }
            else
            {
                var type = estType == EstablishmentType.Academy ? "Academies" : "Maintained";
                var result = (await QueryDBCollectionForCountAsync(criteria, type)).First();
                return result;
            }
        }

        public string GetActiveCollectionByDataGroup(string dataGroup)
        {
            return
                GetActiveCollectionsByDataGroup(dataGroup).OrderByDescending(o => o.Split('-').First()).FirstOrDefault();
        }
       
        public dynamic GetAcademiesByMatNumber(string term, string matNo)
        {
            var collectionName =
                GetActiveCollectionsByDataGroup("Academies")
                    .SingleOrDefault(sod => sod.Split('-').Last() == term.Split(' ').Last());
            try
            {
                var query =
                    $"SELECT c['URN'], c['School Name'] as EstablishmentName, c['Period covered by return'] FROM c WHERE c['MAT Number']='{matNo}'";
                var matches =
                    _client.CreateDocumentQuery<Document>(
                        UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionName), query).ToList();

                dynamic result = new ExpandoObject();
                result.Results = matches;

                return result;
            }
            catch (Exception)
            {
                return new List<Document>();
            }
        }

        public List<string> GetActiveCollectionsByDataGroup(string dataGroup)
        {
            var colls = GetActiveCollections();
            var result = colls.Where(w => w.GetProperty("data-group", "") == dataGroup)
                .Select(s => s.GetProperty("name", ""))
                .ToList();
            return result;
        }

        public List<string> GetActiveTermsByDataGroup(string dataGroup, string format = "{0} - {1}")
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

        private string GetCollectionIdByTermByDataGroup(string term, string dataGroup)
        {
            if (dataGroup == DataGroups.MATDistributed)
            {
                return GetActiveCollectionsByDataGroup(dataGroup)
                    .SingleOrDefault(sod => sod.Split('-')[3] == term.Split(' ').Last());
            }
            return GetActiveCollectionsByDataGroup(dataGroup)
                .SingleOrDefault(sod => sod.Split('-').Last() == term.Split(' ').Last());
        }

        private async Task<IEnumerable<Document>> QueryDBCollectionAsync(BenchmarkCriteria criteria, string type)
        {
            var collectionName = GetLatestActiveTermByDataGroup(type);

            var query = BuildQueryFromBenchmarkCriteria(criteria);

            if (string.IsNullOrEmpty(query))
            {
                return new List<Document>();
            }

            IQueryable<Document> result;

            if (type == "Academies")
            {
                result = _client.CreateDocumentQuery<Document>(
                    UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionName),
                    $"SELECT c['URN'], c['School Name'], c['Type'],  'A' AS FinanceType, c['No Pupils'] FROM c WHERE {query}");
            }
            else
            {
                result = _client.CreateDocumentQuery<Document>(
                    UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionName),
                    $"SELECT c['URN'], c['School Name'], c['Type'], 'M' AS FinanceType, c['No Pupils'] FROM c WHERE {query}");
            }

            return await result.QueryAsync();
        }

        private async Task<IEnumerable<int>> QueryDBCollectionForCountAsync(BenchmarkCriteria criteria, string type)
        {
            var collectionName = GetLatestActiveTermByDataGroup(type);

            var query = BuildQueryFromBenchmarkCriteria(criteria);

            if (string.IsNullOrEmpty(query))
            {
                return new List<int> { 0 };
            }

            var result =
                _client.CreateDocumentQuery<int>(UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionName),
                    $"SELECT VALUE COUNT(c.id) FROM c WHERE {query}");

            return await result.QueryAsync();
        }

        private string BuildQueryFromBenchmarkCriteria(BenchmarkCriteria criteria)
        {
            var queryBuilder = new StringBuilder();
            foreach (var property in (typeof(BenchmarkCriteria)).GetProperties())
            {
                var attribute = property.GetCustomAttributes(typeof(DBFieldAttribute)).FirstOrDefault();

                if (attribute != null && attribute is DBFieldAttribute)
                {
                    var fieldValue = criteria.GetType().GetProperty(property.Name).GetValue(criteria, null);
                    if (fieldValue != null)
                    {
                        var fieldName = (attribute as DBFieldAttribute).Name;
                        var fieldType = (attribute as DBFieldAttribute).Type;
                        switch (fieldType)
                        {
                            case CriteriaFieldComparisonTypes.MIN:
                                queryBuilder.Append($"c['{fieldName}'] >= {fieldValue}");
                                break;
                            case CriteriaFieldComparisonTypes.MAX:
                                queryBuilder.Append($"c['{fieldName}'] <= {fieldValue}");
                                break;
                            case CriteriaFieldComparisonTypes.EQUALTO:
                                if (fieldValue is int)
                                {
                                    queryBuilder.Append($"c['{fieldName}'] = {fieldValue}");
                                }
                                else if (fieldValue is string)
                                {
                                    queryBuilder.Append($"c['{fieldName}'] = '{fieldValue}'");
                                }
                                else if (fieldValue is Array)
                                {
                                    queryBuilder.Append(
                                        $"c['{fieldName}'] IN ('{string.Join("','", fieldValue as string[])}')");
                                }
                                break;
                        }

                        queryBuilder.Append(" AND ");
                    }
                }

            }
            var query = queryBuilder.ToString();
            return string.IsNullOrEmpty(query) ? query : query.Substring(0, query.Length - 5);
        }

        private List<JObject> GetActiveCollections()
        {
            var docs = (List<JObject>)HttpContext.Current.Cache.Get("SFBActiveCollectionList");

            if (docs == null)
            {
                var query = _client.CreateDocumentQuery(UriFactory.CreateDocumentCollectionUri(DatabaseId, "fibre-directory"), "SELECT * FROM c WHERE c['active'] = 'Y'").ToList();

                docs =  query.Select(d => JObject.Parse(d.ToString())).Cast<JObject>().ToList();

                HttpContext.Current.Cache.Insert("SFBActiveCollectionList", docs, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(60));
            }

            return docs;
        }

        private string GetLatestActiveTermByDataGroup(string dataGroup)
        {
            var latestTerm = GetActiveCollections()
                    .Where(w => w.GetProperty("data-group", "") == dataGroup)
                    .OrderByDescending(o => o.GetProperty("term", ""))
                    .FirstOrDefault();
            return latestTerm?.GetProperty("name", "") ?? string.Empty;
        }
    }
}
