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
    public class CosmosDbFinancialDataRepository : IFinancialDataRepository
    {
        private static readonly string DatabaseId = ConfigurationManager.AppSettings["database"];
        private static DocumentClient _client;
        private readonly IDataCollectionManager _dataCollectionManager;

        public CosmosDbFinancialDataRepository(IDataCollectionManager dataCollectionManager)
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

        public Document GetSchoolDataDocument(int urn, string term, EstablishmentType estabType, CentralFinancingType cFinance)
        {
            var dataGroup = estabType.ToDataGroup(cFinance);

            var collectionName = _dataCollectionManager.GetCollectionIdByTermByDataGroup(term, dataGroup);

            if (collectionName == null)
            {
                return null;
            }

            try
            {
                var query = $"SELECT * FROM c WHERE c.URN=@URN";
                SqlQuerySpec querySpec = new SqlQuerySpec(query);
                querySpec.Parameters = new SqlParameterCollection();
                querySpec.Parameters.Add(new SqlParameter($"@URN", urn));
                var documentQuery =
                    _client.CreateDocumentQuery<Document>(
                        UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionName),
                        querySpec);

                var result = documentQuery.ToList().FirstOrDefault();

                if (dataGroup == DataGroups.MATAllocs && result == null)//if nothing found in MAT-Allocs collection try to source it from Academies data
                {
                    return GetSchoolDataDocument(urn, term, estabType, CentralFinancingType.Exclude);
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

        public async Task<IEnumerable<Document>> GetSchoolDataDocumentAsync(int urn, string term, EstablishmentType estabType, CentralFinancingType cFinance)
        {
            var dataGroup = estabType.ToDataGroup(cFinance);

            var collectionName = _dataCollectionManager.GetCollectionIdByTermByDataGroup(term, dataGroup);

            try
            {
                var query = $"SELECT * FROM c WHERE c.URN=@URN";
                SqlQuerySpec querySpec = new SqlQuerySpec(query);
                querySpec.Parameters = new SqlParameterCollection();
                querySpec.Parameters.Add(new SqlParameter($"@URN", urn));
                var documentQuery =
                    _client.CreateDocumentQuery<Document>(
                        UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionName),
                        querySpec);

                return await documentQuery.QueryAsync();

            }
            catch (Exception)
            {
                return null;
            }
        }

        public dynamic GetAcademiesByMatNumber(string term, string matNo)
        {
            var collectionName =
                _dataCollectionManager.GetActiveCollectionsByDataGroup(DataGroups.Academies)
                    .SingleOrDefault(sod => sod.Split('-').Last() == term.Split(' ').Last());
            try
            {
                var query = $"SELECT c['URN'], c['School Name'] as EstablishmentName, c['Period covered by return'] FROM c WHERE c['MATNumber']=@MatNo";
                SqlQuerySpec querySpec = new SqlQuerySpec(query);
                querySpec.Parameters = new SqlParameterCollection();
                querySpec.Parameters.Add(new SqlParameter($"@MatNo", matNo));

                var matches =
                    _client.CreateDocumentQuery<Document>(
                        UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionName), querySpec).ToList();

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
            string dataGroupType = null;

            switch (matFinance)
            {
                case MatFinancingType.TrustOnly:
                    dataGroupType = DataGroups.MATCentral;
                    break;
                case MatFinancingType.TrustAndAcademies:
                    dataGroupType = DataGroups.MATOverview;
                    break;
                case MatFinancingType.AcademiesOnly:
                    dataGroupType = DataGroups.MATTotals;
                    break;
            }

            var collectionName = _dataCollectionManager.GetCollectionIdByTermByDataGroup(term, dataGroupType);

            if (collectionName == null)
            {
                return null;
            }

            var query = $"SELECT * FROM c WHERE c['MATNumber']='{matNo}'";
            SqlQuerySpec querySpec = new SqlQuerySpec(query);
            querySpec.Parameters = new SqlParameterCollection();
            querySpec.Parameters.Add(new SqlParameter($"@MatNo", matNo));

            var res =
                _client.CreateDocumentQuery<Document>(
                    UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionName),
                    querySpec);

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
            string matFinanceType = null;
            switch (matFinance)
            {
                case MatFinancingType.TrustOnly:
                    matFinanceType = DataGroups.MATCentral;
                    break;
                case MatFinancingType.TrustAndAcademies:
                    matFinanceType = DataGroups.MATOverview;
                    break;
                case MatFinancingType.AcademiesOnly:
                    matFinanceType = DataGroups.MATTotals;
                    break;
            }

            var collectionName = _dataCollectionManager.GetCollectionIdByTermByDataGroup(term, matFinanceType);

            try
            {
                var query = $"SELECT * FROM c WHERE c['MATNumber']='{matNo}'";
                SqlQuerySpec querySpec = new SqlQuerySpec(query);
                querySpec.Parameters = new SqlParameterCollection();
                querySpec.Parameters.Add(new SqlParameter($"@MatNo", matNo));

                var documentQuery =
                    _client.CreateDocumentQuery<Document>(
                        UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionName),
                        querySpec);

                return await documentQuery.QueryAsync();
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
                var maintainedSchoolsTask = QueryDBSchoolCollectionAsync(criteria, DataGroups.Maintained);
                var academiesTask = QueryDBSchoolCollectionAsync(criteria, DataGroups.Academies);
                var maintainedSchools = (await maintainedSchoolsTask).ToList();
                var academies = (await academiesTask).ToList();
                maintainedSchools.AddRange(academies);
                return maintainedSchools;
            }
            else
            {                
                return (await QueryDBSchoolCollectionAsync(criteria, estType.ToDataGroup())).ToList();
            }
        }

        public async Task<int> SearchSchoolsCountByCriteriaAsync(BenchmarkCriteria criteria, EstablishmentType estType)
        {
            if (estType == EstablishmentType.All)
            {
                var maintainedSchoolsCountTask = QueryDBSchoolCollectionForCountAsync(criteria, DataGroups.Maintained);
                var academiesCountTask = QueryDBSchoolCollectionForCountAsync(criteria, DataGroups.Academies);
                return (await maintainedSchoolsCountTask).First() + (await academiesCountTask).First();
            }
            else
            {
                var type = estType == EstablishmentType.Academies ? DataGroups.Academies : DataGroups.Maintained;
                var result = (await QueryDBSchoolCollectionForCountAsync(criteria, type)).First();
                return result;
            }
        }

        public async Task<List<Document>> SearchTrustsByCriteriaAsync(BenchmarkCriteria criteria)
        {            
            return (await QueryDBTrustCollectionAsync(criteria, DataGroups.MATOverview)).ToList();
        }

        public async Task<int> SearchTrustCountByCriteriaAsync(BenchmarkCriteria criteria)
        {
            return (await QueryDBTrustCollectionForCountAsync(criteria)).First();
        }

        public async Task<int> GetEstablishmentRecordCountAsync(string term, EstablishmentType estType)
        {
            var collectionName = string.Empty;
            switch (estType)
            {
                case EstablishmentType.Academies:
                    collectionName = _dataCollectionManager.GetCollectionIdByTermByDataGroup(term, DataGroups.MATAllocs);
                    break;
                case EstablishmentType.Maintained:
                    collectionName = _dataCollectionManager.GetCollectionIdByTermByDataGroup(term, DataGroups.Maintained);
                    break;
                case EstablishmentType.MAT:
                    collectionName = _dataCollectionManager.GetCollectionIdByTermByDataGroup(term, DataGroups.MATOverview);
                    break;
            }

            var result =
                _client.CreateDocumentQuery<int>(UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionName), $"SELECT VALUE COUNT(c) FROM c");

            return (await result.QueryAsync()).First();
        }

        private async Task<IEnumerable<Document>> QueryDBSchoolCollectionAsync(BenchmarkCriteria criteria, string dataGroup)
        {
            var collectionName = _dataCollectionManager.GetLatestActiveTermByDataGroup(dataGroup);

            var query = BuildQueryFromBenchmarkCriteria(criteria);

            query = Exclude6Forms(query);

            if (string.IsNullOrEmpty(query))
            {
                return new List<Document>();
            }

            IQueryable<Document> result;

            if (dataGroup == DataGroups.Academies || dataGroup == DataGroups.MATAllocs)
            {
                result = _client.CreateDocumentQuery<Document>(
                    UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionName),
                    $"SELECT c['URN'], c['School Name'], c['Type'],  '{DataGroups.Academies}' AS FinanceType, c['No Pupils'] FROM c WHERE {query}");
            }
            else
            {
                result = _client.CreateDocumentQuery<Document>(
                    UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionName),
                    $"SELECT c['URN'], c['School Name'], c['Type'], '{DataGroups.Maintained}' AS FinanceType, c['No Pupils'] FROM c WHERE {query}");
            }

            return await result.QueryAsync();
        }

        private async Task<IEnumerable<Document>> QueryDBTrustCollectionAsync(BenchmarkCriteria criteria, string dataGroup)
        {
            var collectionName = _dataCollectionManager.GetLatestActiveTermByDataGroup(dataGroup);

            var query = BuildQueryFromBenchmarkCriteria(criteria);

            query = ExcludeSAMATs(query);

            if (string.IsNullOrEmpty(query))
            {
                return new List<Document>();
            }

            IQueryable<Document> result;
            
            result = _client.CreateDocumentQuery<Document>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionName),
                $"SELECT c['MATNumber'], c['TrustOrCompanyName'] FROM c WHERE {query}");
            
            return await result.QueryAsync();
        }

        private async Task<IEnumerable<int>> QueryDBSchoolCollectionForCountAsync(BenchmarkCriteria criteria, string type)
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
                    $"SELECT VALUE COUNT(c) FROM c WHERE {query}");

            return await result.QueryAsync();
        }

        private async Task<IEnumerable<int>> QueryDBTrustCollectionForCountAsync(BenchmarkCriteria criteria)
        {
            var collectionName = _dataCollectionManager.GetLatestActiveTermByDataGroup(DataGroups.MATOverview);

            var query = BuildQueryFromBenchmarkCriteria(criteria);

            //query = AddMembersCriteria(query, criteria);

            query = ExcludeSAMATs(query);

            if (string.IsNullOrEmpty(query))
            {
                return new List<int> { 0 };
            }

            var result =
                _client.CreateDocumentQuery<int>(UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionName),
                    $"SELECT VALUE COUNT(c) FROM c WHERE {query}");

            return await result.QueryAsync();
        }

        private string ExcludeSAMATs(string query)
        {
            return $"{query} AND c.MemberCount > 1";
        }

        private string Exclude6Forms(string query)
        {
            return $"{query} AND c['Type'] != 'Free 16-19'";
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
                        var docName = (attribute as DBFieldAttribute).DocName;
                        var fieldName = (attribute as DBFieldAttribute).Name;
                        var fieldType = (attribute as DBFieldAttribute).Type;
                        switch (fieldType)
                        {
                            case CriteriaFieldComparisonTypes.MIN:
                                if (string.IsNullOrEmpty(docName))
                                {
                                    queryBuilder.Append($"c['{fieldName}'] >= {fieldValue}");
                                }
                                else
                                {
                                    queryBuilder.Append($"c['{docName}']['{fieldName}'] >= {fieldValue}");
                                }
                                break;
                            case CriteriaFieldComparisonTypes.MAX:
                                if (string.IsNullOrEmpty(docName))
                                {
                                    queryBuilder.Append($"c['{fieldName}'] <= {fieldValue}");
                                }
                                else
                                {
                                    queryBuilder.Append($"c['{docName}']['{fieldName}'] <= {fieldValue}");
                                }
                                break;
                            case CriteriaFieldComparisonTypes.EQUALTO:
                                if (fieldValue is int)
                                {
                                    if (string.IsNullOrEmpty(docName))
                                    {
                                        queryBuilder.Append($"c['{fieldName}'] = {fieldValue}");
                                    }
                                    else
                                    {
                                        queryBuilder.Append($"c['{docName}']['{fieldName}'] = {fieldValue}");
                                    }
                                }
                                else if (fieldValue is string)
                                {
                                    if (string.IsNullOrEmpty(docName))
                                    {
                                        queryBuilder.Append($"c['{fieldName}'] = '{fieldValue}'");
                                    }
                                    else
                                    {
                                        queryBuilder.Append($"c['{docName}']['{fieldName}'] = '{fieldValue}'");
                                    }
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