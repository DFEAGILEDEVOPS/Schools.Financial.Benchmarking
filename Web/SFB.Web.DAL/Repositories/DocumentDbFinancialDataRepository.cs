using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using SFB.Web.Common;
using SFB.Web.Common.Attributes;
using SFB.Web.DAL.Helpers;

namespace SFB.Web.DAL.Repositories
{
    public class DocumentDbFinancialDataRepository : IFinancialDataRepository
    {
        private static readonly string DatabaseId = ConfigurationManager.AppSettings["database"];
        private static DocumentClient _client;
        private readonly IDataCollectionManager _dataCollectionManager;

        public DocumentDbFinancialDataRepository(IDataCollectionManager dataCollectionManager)
        {
            _dataCollectionManager = dataCollectionManager;

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

            var collectionName = _dataCollectionManager.GetCollectionIdByTermByDataGroup(term, dataGroup);

            if (collectionName == null)
            {
                return null;
            }

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

        public async Task<IEnumerable<Document>> GetSchoolDataDocumentAsync(string urn, string term, SchoolFinancialType schoolFinancialType, CentralFinancingType cFinance)
        {
            var dataGroup = schoolFinancialType.ToString();
            if (schoolFinancialType == SchoolFinancialType.Academies)
            {
                dataGroup = (cFinance == CentralFinancingType.Include) ? DataGroups.MATDistributed : DataGroups.Academies;
            }

            var collectionName = _dataCollectionManager.GetCollectionIdByTermByDataGroup(term, dataGroup);

            try
            {
                var query =
                    _client.CreateDocumentQuery<Document>(
                        UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionName),
                        $"SELECT * FROM c WHERE c.URN={urn}");

                return await query.QueryAsync();

            }
            catch (Exception)
            {
                return null;
            }
        }

        public dynamic GetAcademiesByMatNumber(string term, string matNo)
        {
            var collectionName =
                _dataCollectionManager.GetActiveCollectionsByDataGroup("Academies")
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

        public Document GetMATDataDocument(string matNo, string term, MatFinancingType matFinance)
        {
            var collectionName = _dataCollectionManager.GetCollectionIdByTermByDataGroup(term, matFinance == MatFinancingType.TrustOnly ? DataGroups.MATCentral : DataGroups.MATOverview);

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

        public async Task<IEnumerable<Document>> GetMATDataDocumentAsync(string matNo, string term, MatFinancingType matFinance)
        {
            var collectionName = _dataCollectionManager.GetCollectionIdByTermByDataGroup(term, matFinance == MatFinancingType.TrustOnly ? DataGroups.MATCentral : DataGroups.MATOverview);

            try
            {
                var query =
                    _client.CreateDocumentQuery<Document>(
                        UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionName),
                        $"SELECT * FROM c WHERE c['MATNumber']='{matNo}'");

                return await query.QueryAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<Document>> SearchSchoolsByCriteriaAsync(BenchmarkCriteria criteria, EstablishmentType estType)
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

        public async Task<int> SearchSchoolsCountByCriteriaAsync(BenchmarkCriteria criteria, EstablishmentType estType)
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

        private async Task<IEnumerable<Document>> QueryDBCollectionAsync(BenchmarkCriteria criteria, string type)
        {
            var collectionName = _dataCollectionManager.GetLatestActiveTermByDataGroup(type);

            var query = BuildQueryFromBenchmarkCriteria(criteria);

            query = Exclude6Forms(query);

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

        private string Exclude6Forms(string query)
        {
            return $"{query} AND c['Type'] != 'Free 16-19'";
        }

        private async Task<IEnumerable<int>> QueryDBCollectionForCountAsync(BenchmarkCriteria criteria, string type)
        {
            var collectionName = _dataCollectionManager.GetLatestActiveTermByDataGroup(type);

            var query = BuildQueryFromBenchmarkCriteria(criteria);

            query = Exclude6Forms(query);

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
    }
}