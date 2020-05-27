using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SFB.Web.ApplicationCore.Attributes;
using SFB.Web.ApplicationCore.Entities;
using SFB.Web.ApplicationCore.DataAccess;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Helpers.Constants;
using SFB.Web.ApplicationCore.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using SFB.Web.Infrastructure.Logging;

namespace SFB.Web.Infrastructure.Repositories
{
    public class CosmosDbFinancialDataRepository : AppInsightsLoggable, IFinancialDataRepository
    {
        private readonly string _databaseId;
        private static CosmosClient _client;
        private readonly IDataCollectionManager _dataCollectionManager;

        public CosmosDbFinancialDataRepository(IDataCollectionManager dataCollectionManager, ILogManager logManager) : base(logManager)
        {
            _dataCollectionManager = dataCollectionManager;

            var clientBuilder = new CosmosClientBuilder(ConfigurationManager.AppSettings["endpoint"],
                ConfigurationManager.AppSettings["authKey"]);

            _client = clientBuilder.WithConnectionModeDirect().Build();

            _databaseId = ConfigurationManager.AppSettings["database"];
        }

        public CosmosDbFinancialDataRepository(IDataCollectionManager dataCollectionManager, CosmosClient cosmosClient, string databaseId, ILogManager logManager) : base(logManager)
        {
            _dataCollectionManager = dataCollectionManager;                      

            _client = cosmosClient;

            _databaseId = databaseId;
        }

        public async Task<SchoolTrustFinancialDataObject> GetSchoolFinancialDataObjectAsync(int urn, string term, EstablishmentType estabType, CentralFinancingType cFinance)
        {
            var dataGroup = estabType.ToDataGroup(cFinance);

            var collectionName = await _dataCollectionManager.GetCollectionIdByTermByDataGroupAsync(term, dataGroup);

            var container = _client.GetContainer(_databaseId, collectionName);

            if (collectionName == null)
            {
                return null;
            }

            var queryString = $"SELECT * FROM c WHERE c.{SchoolTrustFinanceDataFieldNames.URN}=@URN";

            var queryDefinition = new QueryDefinition(queryString)
                .WithParameter($"@URN", urn);

            try
            {
                var feedIterator = container.GetItemQueryIterator<SchoolTrustFinancialDataObject>(queryDefinition, null);
                var result = (await feedIterator.ReadNextAsync()).FirstOrDefault();

                if (dataGroup == DataGroups.MATAllocs && result == null)//if nothing found in MAT-Allocs collection try to source it from Academies data
                {
                    return await GetSchoolFinancialDataObjectAsync(urn, term, estabType, CentralFinancingType.Exclude);
                }

                if (result != null && result.DidNotSubmit)//School did not submit finance, return & display none in the charts
                {
                    return null;
                }

                return result;

            }
            catch (Exception ex)
            {
                if (term.Contains(_dataCollectionManager.GetLatestFinancialDataYearPerEstabTypeAsync(estabType).ToString()))
                {
                    var errorMessage = $"{collectionName} could not be loaded! : {ex.Message} : {queryDefinition.QueryText}";
                    base.LogException(ex, errorMessage);
                }
                return null;
            }
        }

        public async Task<SchoolTrustFinancialDataObject> GetSchoolFinanceDataObjectAsync(int urn, string term, EstablishmentType estabType, CentralFinancingType cFinance)
        {
            var dataGroup = estabType.ToDataGroup(cFinance);

            string collectionName = await _dataCollectionManager.GetCollectionIdByTermByDataGroupAsync(term, dataGroup);

            var container = _client.GetContainer(_databaseId, collectionName);

            var queryString = $"SELECT * FROM c WHERE c.{SchoolTrustFinanceDataFieldNames.URN}=@URN";
            var queryDefinition = new QueryDefinition(queryString)
                  .WithParameter($"@URN", urn);

            try
            {
                var feedIterator = container.GetItemQueryIterator<SchoolTrustFinancialDataObject>(queryDefinition, null);
                return (await feedIterator.ReadNextAsync()).FirstOrDefault();

            }
            catch (Exception ex)
            {
                if (term.Contains(_dataCollectionManager.GetLatestFinancialDataYearPerEstabTypeAsync(estabType).ToString()))
                {
                    var errorMessage = $"{collectionName} could not be loaded! : {ex.Message} : {queryDefinition.QueryText}";
                    base.LogException(ex, errorMessage);
                }
                return null;
            }
        }

        public async Task<List<AcademiesContextualDataObject>> GetAcademiesContextualDataObjectAsync(string term, int companyNo)
        {
            var collectionName = (await _dataCollectionManager.GetActiveCollectionsByDataGroupAsync(DataGroups.Academies))
                    .SingleOrDefault(sod => sod.Split('-').Last() == term.Split(' ').Last());

            var container = _client.GetContainer(_databaseId, collectionName);

            var queryString = $"SELECT c['{SchoolTrustFinanceDataFieldNames.URN}'], " +
                $"c['{SchoolTrustFinanceDataFieldNames.SCHOOL_NAME}'] as EstablishmentName, " +
                $"c['{SchoolTrustFinanceDataFieldNames.PERIOD_COVERED_BY_RETURN}'], " +
                $"c['{SchoolTrustFinanceDataFieldNames.TRUST_COMPANY_NAME}']" +
                $"FROM c WHERE c['{SchoolTrustFinanceDataFieldNames.COMPANY_NUMBER}']=@companyNo";

            var queryDefinition = new QueryDefinition(queryString)
                .WithParameter($"@companyNo", companyNo);

            try
            {
                var results = new List<AcademiesContextualDataObject>();

                var feedIterator = container.GetItemQueryIterator<AcademiesContextualDataObject>(queryDefinition, null);

                while (feedIterator.HasMoreResults)
                {
                    foreach (var item in await feedIterator.ReadNextAsync())
                    {
                        results.Add(item);
                    }
                }

                return results;
            }
            catch (Exception ex)
            {
                var errorMessage = $"{collectionName} could not be loaded! : {ex.Message} : {queryDefinition.QueryText}";
                base.LogException(ex, errorMessage);

                return null;
            }
        }

        public async Task<SchoolTrustFinancialDataObject> GetTrustFinancialDataObjectAsync(int companyNo, string term, MatFinancingType matFinance)
        {
            var dataGroup = EstablishmentType.MAT.ToDataGroup(matFinance);

            var collectionName = await _dataCollectionManager.GetCollectionIdByTermByDataGroupAsync(term, dataGroup);

            if (collectionName == null)
            {
                return null;
            }

            var container = _client.GetContainer(_databaseId, collectionName);

            var queryString = $"SELECT * FROM c WHERE c['{SchoolTrustFinanceDataFieldNames.COMPANY_NUMBER}']=@companyNo";

            var queryDefinition = new QueryDefinition(queryString)
                .WithParameter($"@companyNo", companyNo);

            var feedIterator = container.GetItemQueryIterator<SchoolTrustFinancialDataObject>(queryDefinition, null);
            
            try
            {
                var result = (await feedIterator.ReadNextAsync()).FirstOrDefault();

                if (result != null && result.DidNotSubmit)
                {
                    var emptyObj = new SchoolTrustFinancialDataObject();
                    emptyObj.DidNotSubmit = true;
                    return emptyObj;
                }
                return result;
            }
            catch (Exception ex)
            {
                if (term.Contains(_dataCollectionManager.GetLatestFinancialDataYearPerEstabTypeAsync(EstablishmentType.MAT).ToString()))
                {
                    var errorMessage = $"{collectionName} could not be loaded! : {ex.Message} : {queryDefinition.QueryText}";
                    base.LogException(ex, errorMessage);
                }
                return null;
            }
        }

        public async Task<List<SchoolTrustFinancialDataObject>> GetMultipleTrustFinancialDataObjectsAsync(List<int> companyNoList, string term, MatFinancingType matFinance)
        {
            var dataGroup = EstablishmentType.MAT.ToDataGroup(matFinance);

            var collectionName = await _dataCollectionManager.GetCollectionIdByTermByDataGroupAsync(term, dataGroup);

            if (collectionName == null)
            {
                return null;
            }

            var container = _client.GetContainer(_databaseId, collectionName);

            var queryString = $"SELECT * FROM c WHERE c['{SchoolTrustFinanceDataFieldNames.COMPANY_NUMBER}'] in ({string.Join(",", companyNoList)})";

            var queryDefinition = new QueryDefinition(queryString);

            var feedIterator = container.GetItemQueryIterator<SchoolTrustFinancialDataObject>(queryDefinition, null);

            var resultsList = new List<SchoolTrustFinancialDataObject>();

            while (feedIterator.HasMoreResults)
            {
                foreach (var item in await feedIterator.ReadNextAsync())
                {
                    resultsList.Add(item);
                }
            }

            try
            {
                resultsList.ForEach(result => {
                    if (result.DidNotSubmit)
                    {
                        var emptyObj = new SchoolTrustFinancialDataObject();
                        emptyObj.DidNotSubmit = true;
                        result = emptyObj;
                    }
                });               
            }
            catch (Exception ex)
            {
                var errorMessage = $"{collectionName} could not be loaded! : {ex.Message} : URNs = {string.Join(",", companyNoList)}";
                base.LogException(ex, errorMessage);
                throw new ApplicationException($"One or more documents could not be loaded from {collectionName} : URNs = {string.Join(",", companyNoList)}");
            }

            return resultsList;
        }

        public async Task<SchoolTrustFinancialDataObject> GetTrustFinancialDataObjectByMatNameAsync(string matName, string term, MatFinancingType matFinance)
        {
            var dataGroup = EstablishmentType.MAT.ToDataGroup(matFinance);

            var collectionName = await _dataCollectionManager.GetCollectionIdByTermByDataGroupAsync(term, dataGroup);

            if (collectionName == null)
            {
                return null;
            }

            var container = _client.GetContainer(_databaseId, collectionName);

            var queryString = $"SELECT * FROM c WHERE c['{SchoolTrustFinanceDataFieldNames.TRUST_COMPANY_NAME}']=@matName";

            var queryDefinition = new QueryDefinition(queryString)
                .WithParameter($"@matName", matName);

            var feedIterator = container.GetItemQueryIterator<SchoolTrustFinancialDataObject>(queryDefinition, null);

            try
            {
                var result = (await feedIterator.ReadNextAsync()).FirstOrDefault();

                if (result != null && result.DidNotSubmit)
                {
                    var emptyObj = new SchoolTrustFinancialDataObject();
                    emptyObj.DidNotSubmit = true;
                    return emptyObj;
                }
                return result;
            }
            catch (Exception ex)
            {
                if (term.Contains(_dataCollectionManager.GetLatestFinancialDataYearPerEstabTypeAsync(EstablishmentType.MAT).ToString()))
                {
                    var errorMessage = $"{collectionName} could not be loaded! : {ex.Message} : {queryDefinition.QueryText}";
                    base.LogException(ex, errorMessage);
                }
                return null;
            }
        }

        //public async Task<IEnumerable<SchoolTrustFinancialDataObject>> GetTrustFinancialDataObjectAsync(int companyNo, string term, MatFinancingType matFinance)
        //{
        //    var dataGroup = EstablishmentType.MAT.ToDataGroup(matFinance);

        //    var collectionName = await _dataCollectionManager.GetCollectionIdByTermByDataGroupAsync(term, dataGroup);

        //    var query = $"SELECT * FROM c WHERE c['{SchoolTrustFinanceDataFieldNames.COMPANY_NUMBER}']=@companyNo";
        //    SqlQuerySpec querySpec = new SqlQuerySpec(query);
        //    querySpec.Parameters = new SqlParameterCollection();
        //    querySpec.Parameters.Add(new SqlParameter($"@companyNo", companyNo));

        //    try
        //    {
        //        var documentQuery =
        //            _client.CreateDocumentQuery<SchoolTrustFinancialDataObject>(
        //                UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionName),
        //                querySpec);

        //        return await documentQuery.QueryAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        if (term.Contains(_dataCollectionManager.GetLatestFinancialDataYearPerEstabTypeAsync(EstablishmentType.MAT).ToString()))
        //        {
        //            var errorMessage = $"{collectionName} could not be loaded! : {ex.Message} : {querySpec.Parameters[0].Name} = {querySpec.Parameters[0].Value}";
        //            base.LogException(ex, errorMessage);
        //        }
        //        return null;
        //    }
        //}

        public async Task<List<SchoolTrustFinancialDataObject>> SearchSchoolsByCriteriaAsync(BenchmarkCriteria criteria, EstablishmentType estType, bool excludePartial = false)
        {
            if (estType == EstablishmentType.All)
            {
                var maintainedSchoolsTask = QueryDBSchoolCollectionAsync(criteria, DataGroups.Maintained, excludePartial);
                var academiesTask = QueryDBSchoolCollectionAsync(criteria, DataGroups.Academies, excludePartial);
                var maintainedSchools = (await maintainedSchoolsTask).ToList();
                var academies = (await academiesTask).ToList();
                maintainedSchools.AddRange(academies);
                return maintainedSchools;
            }
            else
            {
                return (await QueryDBSchoolCollectionAsync(criteria, estType.ToDataGroup(), excludePartial)).ToList();
            }
        }

        public async Task<int> SearchSchoolsCountByCriteriaAsync(BenchmarkCriteria criteria, EstablishmentType estType, bool excludePartial = false)
        {
            if (estType == EstablishmentType.All)
            {
                var maintainedSchoolsCountTask = QueryDBSchoolCollectionForCountAsync(criteria, DataGroups.Maintained, excludePartial);
                var academiesCountTask = QueryDBSchoolCollectionForCountAsync(criteria, DataGroups.Academies, excludePartial);
                return (await maintainedSchoolsCountTask) + (await academiesCountTask);
            }
            else
            {
                var type = estType == EstablishmentType.Academies ? DataGroups.Academies : DataGroups.Maintained;
                var result = (await QueryDBSchoolCollectionForCountAsync(criteria, type, excludePartial));
                return result;
            }
        }

        public async Task<List<SchoolTrustFinancialDataObject>> SearchTrustsByCriteriaAsync(BenchmarkCriteria criteria)
        {
            return (await QueryDBTrustCollectionAsync(criteria, DataGroups.MATOverview)).ToList();
        }

        public async Task<int> SearchTrustCountByCriteriaAsync(BenchmarkCriteria criteria)
        {
            return (await QueryDBTrustCollectionForCountAsync(criteria));
        }

        public async Task<int> GetEstablishmentRecordCountAsync(string term, EstablishmentType estType)
        {
            var collectionName = string.Empty;
            switch (estType)
            {
                case EstablishmentType.Academies:
                    collectionName = await _dataCollectionManager.GetCollectionIdByTermByDataGroupAsync(term, DataGroups.MATAllocs);
                    break;
                case EstablishmentType.Maintained:
                    collectionName = await _dataCollectionManager.GetCollectionIdByTermByDataGroupAsync(term, DataGroups.Maintained);
                    break;
                case EstablishmentType.MAT:
                    collectionName = await _dataCollectionManager.GetCollectionIdByTermByDataGroupAsync(term, DataGroups.MATOverview);
                    break;
            }

            var container = _client.GetContainer(_databaseId, collectionName);

            var queryDefinition = new QueryDefinition("SELECT VALUE COUNT(c) FROM c");

            var feedIterator = container.GetItemQueryIterator<int>(queryDefinition, null);

            var result = (await feedIterator.ReadNextAsync()).FirstOrDefault();

            return result;
        }

        private async Task<IEnumerable<SchoolTrustFinancialDataObject>> QueryDBSchoolCollectionAsync(BenchmarkCriteria criteria, string dataGroup, bool excludePartial = false)
        {
            var collectionName = await _dataCollectionManager.GetLatestActiveCollectionIdByDataGroupAsync(dataGroup);

            var query = BuildQueryFromBenchmarkCriteria(criteria);

            if(excludePartial)
            {
                query = ExcludePartials(query);
            }

            if (string.IsNullOrEmpty(query))
            {
                return new List<SchoolTrustFinancialDataObject>();
            }

            var container = _client.GetContainer(_databaseId, collectionName);

            var resultList = new List<SchoolTrustFinancialDataObject>();

            try
            {
                string queryString;

                if (dataGroup == DataGroups.Academies || dataGroup == DataGroups.MATAllocs)
                {
                        queryString =
                        $"SELECT c['{SchoolTrustFinanceDataFieldNames.URN}'], " +
                        $"c['{SchoolTrustFinanceDataFieldNames.SCHOOL_NAME}'], " +
                        $"c['{SchoolTrustFinanceDataFieldNames.SCHOOL_TYPE}'], " +
                        $"'{DataGroups.Academies}' AS {SchoolTrustFinanceDataFieldNames.FINANCE_TYPE}, " +
                        $"c['{SchoolTrustFinanceDataFieldNames.NO_PUPILS}'], " +
                        $"c['{SchoolTrustFinanceDataFieldNames.SCHOOL_PHASE}'], " +
                        $"c['{SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE}'], " +
                        $"c['{SchoolTrustFinanceDataFieldNames.KS2_PROGRESS}'], " +
                        $"c['{SchoolTrustFinanceDataFieldNames.PROGRESS_8_MEASURE}'], " +
                        $"c['{SchoolTrustFinanceDataFieldNames.PROGRESS_8_BANDING}'], " +
                        $"c['{SchoolTrustFinanceDataFieldNames.TOTAL_EXP_PP}'] " +
                        $"FROM c WHERE {query}";
                }
                else
                {
                        queryString =
                        $"SELECT c['{SchoolTrustFinanceDataFieldNames.URN}'], " +
                        $"c['{SchoolTrustFinanceDataFieldNames.SCHOOL_NAME}'], " +
                        $"c['{SchoolTrustFinanceDataFieldNames.SCHOOL_TYPE}'], " +
                        $"'{DataGroups.Maintained}' AS {SchoolTrustFinanceDataFieldNames.FINANCE_TYPE}, " +
                        $"c['{SchoolTrustFinanceDataFieldNames.NO_PUPILS}'], " +
                        $"c['{SchoolTrustFinanceDataFieldNames.SCHOOL_PHASE}'], " +
                        $"c['{SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE}'], " +
                        $"c['{SchoolTrustFinanceDataFieldNames.KS2_PROGRESS}'], " +
                        $"c['{SchoolTrustFinanceDataFieldNames.PROGRESS_8_MEASURE}'], " +
                        $"c['{SchoolTrustFinanceDataFieldNames.PROGRESS_8_BANDING}'], " +
                        $"c['{SchoolTrustFinanceDataFieldNames.TOTAL_EXP_PP}'] " +
                        $"FROM c WHERE {query}";
                }

                var queryDefinition = new QueryDefinition(queryString);

                var feedIterator = container.GetItemQueryIterator<SchoolTrustFinancialDataObject>(queryDefinition, null);
                 
                while (feedIterator.HasMoreResults)
                {
                    foreach (var item in await feedIterator.ReadNextAsync())
                    {
                        resultList.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"{collectionName} could not be loaded! : {ex.Message} : {query}";
                base.LogException(ex, errorMessage);

                return null;
            }

            return resultList;
        }

        private async Task<IEnumerable<SchoolTrustFinancialDataObject>> QueryDBTrustCollectionAsync(BenchmarkCriteria criteria, string dataGroup)
        {
            var collectionName = await _dataCollectionManager.GetLatestActiveCollectionIdByDataGroupAsync(dataGroup);

            var container = _client.GetContainer(_databaseId, collectionName);

            var query = BuildQueryFromBenchmarkCriteria(criteria);

            query = ExcludeSAMATs(query);

            if (string.IsNullOrEmpty(query))
            {
                return new List<SchoolTrustFinancialDataObject>();
            }
                       
            var resultList = new List<SchoolTrustFinancialDataObject>();

            try
            {
                var queryString = $"SELECT c['{SchoolTrustFinanceDataFieldNames.COMPANY_NUMBER}'], " +
                    $"c['{SchoolTrustFinanceDataFieldNames.UID}'], " +
                    $"c['{SchoolTrustFinanceDataFieldNames.TRUST_COMPANY_NAME}'] " +
                    $"FROM c WHERE {query}";

                var queryDefinition = new QueryDefinition(queryString);

                var feedIterator = container.GetItemQueryIterator<SchoolTrustFinancialDataObject>(queryDefinition, null);

                while (feedIterator.HasMoreResults)
                {
                    foreach (var item in await feedIterator.ReadNextAsync())
                    {
                        resultList.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"{collectionName} could not be loaded! : {ex.Message} : {query}";
                base.LogException(ex, errorMessage);

                return null;
            }

            return resultList;
        }

        private async Task<int> QueryDBSchoolCollectionForCountAsync(BenchmarkCriteria criteria, string type, bool excludePartial = false)
        {
            var collectionName = await _dataCollectionManager.GetLatestActiveCollectionIdByDataGroupAsync(type);

            var container = _client.GetContainer(_databaseId, collectionName);

            var query = BuildQueryFromBenchmarkCriteria(criteria);

            if (excludePartial)
            {
                query = ExcludePartials(query);
            }

            var queryString = $"SELECT VALUE COUNT(c) FROM c WHERE {query}";

            if (string.IsNullOrEmpty(query))
            {
                queryString = $"SELECT VALUE COUNT(c) FROM c";
            }            

            var queryDefinition = new QueryDefinition(queryString);

            var feedIterator = container.GetItemQueryIterator<int>(queryDefinition, null);

            return (await feedIterator.ReadNextAsync()).FirstOrDefault();
        }

        private async Task<int> QueryDBTrustCollectionForCountAsync(BenchmarkCriteria criteria)
        {
            var collectionName = await _dataCollectionManager.GetLatestActiveCollectionIdByDataGroupAsync(DataGroups.MATOverview);

            var container = _client.GetContainer(_databaseId, collectionName);

            var query = BuildQueryFromBenchmarkCriteria(criteria);

            query = ExcludeSAMATs(query);

            if (string.IsNullOrEmpty(query))
            {
                return 0;
            }

            var queryString = $"SELECT VALUE COUNT(c) FROM c WHERE {query}";

            var queryDefinition = new QueryDefinition(queryString);

            var feedIterator = container.GetItemQueryIterator<int>(queryDefinition, null);

            return (await feedIterator.ReadNextAsync()).FirstOrDefault();
        }

        private string ExcludeSAMATs(string query)
        {
            return $"{query} AND c.{SchoolTrustFinanceDataFieldNames.MEMBER_COUNT} > 1";
        }

        private string ExcludePartials(string query)
        {
            if(string.IsNullOrEmpty(query))
            {
                return $"c['{SchoolTrustFinanceDataFieldNames.PERIOD_COVERED_BY_RETURN}'] = 12";
            }
            return $"{query} AND c['{SchoolTrustFinanceDataFieldNames.PERIOD_COVERED_BY_RETURN}'] = 12";
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