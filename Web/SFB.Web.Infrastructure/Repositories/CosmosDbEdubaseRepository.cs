using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Cosmos.Scripts;
using SFB.Web.ApplicationCore.DataAccess;
using SFB.Web.ApplicationCore.Entities;
using SFB.Web.ApplicationCore.Helpers.Constants;
using SFB.Web.Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFB.Web.Infrastructure.Repositories
{
    public class CosmosDbEdubaseRepository : AppInsightsLoggable, IEdubaseRepository
    {
        private readonly string _databaseId;
        private static CosmosClient _client;
        private IDataCollectionManager _dataCollectionManager;

        public CosmosDbEdubaseRepository(IDataCollectionManager dataCollectionManager, ILogManager logManager) : base(logManager)
        {
            _dataCollectionManager = dataCollectionManager;

            var clientBuilder = new CosmosClientBuilder(ConfigurationManager.AppSettings["endpoint"], ConfigurationManager.AppSettings["authKey"]);

            _client = clientBuilder.WithConnectionModeDirect().Build();

            _databaseId = ConfigurationManager.AppSettings["database"];

            _ = CreateUDFsAsync();
        }

        public CosmosDbEdubaseRepository(IDataCollectionManager dataCollectionManager, CosmosClient cosmosClient, string databaseId, ILogManager logManager) : base(logManager)
        {
            _dataCollectionManager = dataCollectionManager;

            _client = cosmosClient;

            _databaseId = databaseId;

            _ = CreateUDFsAsync();
        }

        private async Task CreateUDFsAsync()
        {
            var collectionName = await _dataCollectionManager.GetLatestActiveCollectionByDataGroupAsync(DataGroups.Edubase);

            var container = _client.GetContainer(_databaseId, collectionName);

            await container.Scripts.CreateUserDefinedFunctionAsync(new UserDefinedFunctionProperties
            {
                Id = "PARSE_FINANCIAL_TYPE_CODE",
                Body = @"function(code) {
                    switch (code) {
                       case 'A':
                           return 'Academies';
                       case 'M':
                           return 'Maintained';
                       default:
                           return 'Maintained';
                        }
                    }"
            });
        }

        public async Task<EdubaseDataObject> GetSchoolDataObjectByUrnAsync(int urn)
        {
            var schoolDataObjects = await GetSchoolDataObjectByIdAsync(new Dictionary<string, object> { { SchoolTrustFinanceDataFieldNames.URN, urn } });
            return schoolDataObjects.FirstOrDefault();
        }

        public async Task<List<EdubaseDataObject>> GetMultipleSchoolDataObjectsByUrnsAsync(List<int> urns)
        {
            return await GetMultipleSchoolDataObjectsByIdsAsync(SchoolTrustFinanceDataFieldNames.URN, urns);
        }

        public async Task<List<EdubaseDataObject>> GetSchoolsByLaEstabAsync(string laEstab, bool openOnly)
        {
            var parameters = new Dictionary<string, object>
            {
                {EdubaseDataFieldNames.LA_CODE, Int32.Parse(laEstab.Substring(0, 3))},
                {EdubaseDataFieldNames.ESTAB_NO, Int32.Parse(laEstab.Substring(3))}
            };

            if (openOnly)
            {
                parameters.Add(EdubaseDataFieldNames.ESTAB_STATUS, "Open");
            }

            return await GetSchoolDataObjectByIdAsync(parameters);
        }

        public async Task<List<int>> GetAllSchoolUrnsAsync()
        {
            var queryString = $"SELECT VALUE c.URN FROM c";

            var collectionName = await _dataCollectionManager.GetLatestActiveCollectionByDataGroupAsync(DataGroups.Edubase);

            var container = _client.GetContainer(_databaseId, collectionName);
            
            var query = container.GetItemQueryIterator<int>(new QueryDefinition(queryString));

            List<int> results = new List<int>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<IEnumerable<EdubaseDataObject>> GetAcademiesByCompanyNoAsync(int companyNo)
        {
            var collectionName = await _dataCollectionManager.GetLatestActiveCollectionByDataGroupAsync(DataGroups.Edubase);

            var container = _client.GetContainer(_databaseId, collectionName);

            var results = new List<EdubaseDataObject>();

            var queryString = $"SELECT c['{EdubaseDataFieldNames.URN}'], " +
                $"c['{EdubaseDataFieldNames.ESTAB_NAME}'], " +
                $"c['{EdubaseDataFieldNames.OVERALL_PHASE}'], " +
                $"c['{EdubaseDataFieldNames.COMPANY_NUMBER}'] " +
                $"FROM c WHERE c.{EdubaseDataFieldNames.COMPANY_NUMBER}=@CompanyNo " +
                $"AND c.{EdubaseDataFieldNames.FINANCE_TYPE} = 'A' " +
                $"AND c.{EdubaseDataFieldNames.ESTAB_STATUS_IN_YEAR} = 'Open'";

            var queryDefinition = new QueryDefinition(queryString)
                .WithParameter($"@CompanyNo", companyNo);

            try
            {
                var feedIterator = container.GetItemQueryIterator<EdubaseDataObject>(queryDefinition, null);

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

        public async Task<int> GetAcademiesCountByCompanyNoAsync(int companyNo)
        {
            var collectionName = await _dataCollectionManager.GetLatestActiveCollectionByDataGroupAsync(DataGroups.Edubase);

            var container = _client.GetContainer(_databaseId, collectionName);

            var queryString = $"SELECT VALUE COUNT(c) " +
                $"FROM c WHERE c.{EdubaseDataFieldNames.COMPANY_NUMBER}=@CompanyNo " +
                $"AND c.{EdubaseDataFieldNames.FINANCE_TYPE} = 'A' " +
                $"AND c.{EdubaseDataFieldNames.ESTAB_STATUS_IN_YEAR} = 'Open'";

            var queryDefinition = new QueryDefinition(queryString)
                .WithParameter($"@CompanyNo", companyNo);

            try
            {
                var feedIterator = container.GetItemQueryIterator<int>(queryDefinition, null);
                return (await feedIterator.ReadNextAsync()).First();
            }
            catch (Exception ex)
            {
                var errorMessage = $"{collectionName} could not be loaded! : {ex.Message} : {queryDefinition.QueryText}";
                base.LogException(ex, errorMessage);
                return 0;
            }
        }

        #region Private methods

        private async Task<List<EdubaseDataObject>> GetSchoolDataObjectByIdAsync(Dictionary<string, object> fields)
        {
            var collectionName = await _dataCollectionManager.GetLatestActiveCollectionByDataGroupAsync(DataGroups.Edubase);

            var container = _client.GetContainer(_databaseId, collectionName);

            var sb = new StringBuilder();
            foreach (var field in fields)
            {
                sb.Append($"c.{field.Key}=@{field.Key} AND ");
            }

            var where = sb.ToString().Substring(0, sb.ToString().Length - 5);

            var queryString =
                $"SELECT c['{EdubaseDataFieldNames.URN}'], c['{EdubaseDataFieldNames.ESTAB_NAME}'], c['{EdubaseDataFieldNames.OVERALL_PHASE}'], c['{EdubaseDataFieldNames.PHASE_OF_EDUCATION}'], c['{EdubaseDataFieldNames.TYPE_OF_ESTAB}'], c['{EdubaseDataFieldNames.STREET}'], c['{EdubaseDataFieldNames.TOWN}'], c['{EdubaseDataFieldNames.LOCATION}'], c['{EdubaseDataFieldNames.POSTCODE}'], c['{EdubaseDataFieldNames.TRUSTS}'], c['{EdubaseDataFieldNames.MAT_NUMBER}'], c['{EdubaseDataFieldNames.COMPANY_NUMBER}'], " +
                $"c['{EdubaseDataFieldNames.LA_CODE}'], c['{EdubaseDataFieldNames.ESTAB_NO}'], c['{EdubaseDataFieldNames.TEL_NO}'], c['{EdubaseDataFieldNames.NO_PUPIL}'], c['{EdubaseDataFieldNames.STAT_LOW}'], c['{EdubaseDataFieldNames.STAT_HIGH}'], c['{EdubaseDataFieldNames.HEAD_FIRST_NAME}'], " +
                $"c['{EdubaseDataFieldNames.HEAD_LAST_NAME}'], c['{EdubaseDataFieldNames.HAS_NURSERY}'], c['{EdubaseDataFieldNames.OFFICIAL_6_FORM}'], c['{EdubaseDataFieldNames.SCHOOL_WEB_SITE}'], c['{EdubaseDataFieldNames.OFSTED_RATING}'], c['{EdubaseDataFieldNames.OFSTE_LAST_INSP}'], udf.PARSE_FINANCIAL_TYPE_CODE(c['{EdubaseDataFieldNames.FINANCE_TYPE}']) AS {EdubaseDataFieldNames.FINANCE_TYPE}, c['{EdubaseDataFieldNames.OPEN_DATE}'], c['{EdubaseDataFieldNames.CLOSE_DATE}'] FROM c WHERE {where}";

            var queryDefinition = new QueryDefinition(queryString);
            foreach (var field in fields)
            {
                queryDefinition = queryDefinition.WithParameter($"@{field.Key}", field.Value);
            }

            var results = new List<EdubaseDataObject>();

            try
            {
                var feedIterator = container.GetItemQueryIterator<EdubaseDataObject>(queryDefinition, null);

                while (feedIterator.HasMoreResults)
                {
                    foreach (var item in await feedIterator.ReadNextAsync())
                    {
                        results.Add(item);
                    }
                }

                if (results.Count == 0)
                {
                    throw new ApplicationException("School document not found in Edubase collection!");
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

        private async Task<List<EdubaseDataObject>> GetMultipleSchoolDataObjectsByIdsAsync(string fieldName, List<int> ids)
        {
            var collectionName = await _dataCollectionManager.GetLatestActiveCollectionByDataGroupAsync(DataGroups.Edubase);

            var container = _client.GetContainer(_databaseId, collectionName);

            var sb = new StringBuilder();
            ids.ForEach(u => sb.Append(u + ","));

            var queryString = $"SELECT c['{EdubaseDataFieldNames.URN}'], " +
                $"c['{EdubaseDataFieldNames.ESTAB_NAME}'], " +
                $"c['{EdubaseDataFieldNames.OVERALL_PHASE}'], " +
                $"c['{EdubaseDataFieldNames.TYPE_OF_ESTAB}'], " +
                $"c['{EdubaseDataFieldNames.STREET}'], " +
                $"c['{EdubaseDataFieldNames.TOWN}'], " +
                $"c['{EdubaseDataFieldNames.POSTCODE}'], " +
                $"c['{EdubaseDataFieldNames.TEL_NO}'], " +
                $"c['{EdubaseDataFieldNames.HEAD_FIRST_NAME}'], " +
                $"c['{EdubaseDataFieldNames.HEAD_LAST_NAME}'], " +
                $"c['{EdubaseDataFieldNames.LA_CODE}'], " +
                $"c['{EdubaseDataFieldNames.NO_PUPIL}'], " +
                $"c['{EdubaseDataFieldNames.RELIGIOUS_CHARACTER}'], " +
                $"c['{EdubaseDataFieldNames.OFSTED_RATING}'], " +
                $"c['{EdubaseDataFieldNames.LOCATION}'], "+
                $"udf.PARSE_FINANCIAL_TYPE_CODE(c['{EdubaseDataFieldNames.FINANCE_TYPE}']) AS {EdubaseDataFieldNames.FINANCE_TYPE} " +
                $"FROM c WHERE c.{fieldName} IN ({sb.ToString().TrimEnd(',')})";

            var results = new List<EdubaseDataObject>();
            try
            {
                var query = container.GetItemQueryIterator<EdubaseDataObject>(new QueryDefinition(queryString));
                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();

                    results.AddRange(response.ToList());
                }
                if (results.Count < ids.Count)
                {
                    throw new Newtonsoft.Json.JsonSerializationException();
                }
                return results;
            }
            catch (Exception ex)
            {
                var errorMessage = $"{collectionName} could not be loaded! : {ex.Message} : URNs = {sb.ToString().TrimEnd(',')}";
                base.LogException(ex, errorMessage);
                throw new ApplicationException($"One or more documents could not be loaded from {collectionName} : URNs = {sb.ToString().TrimEnd(',')}");
            }
        }

        #endregion
    }
}
