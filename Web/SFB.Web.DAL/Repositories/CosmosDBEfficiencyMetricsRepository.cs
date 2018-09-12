using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using SFB.Web.Common;
using SFB.Web.Common.DataObjects;
using SFB.Web.DAL.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SFB.Web.DAL.Repositories
{
    public class CosmosDBEfficiencyMetricsRepository : AppInsightsLoggable, IEfficiencyMetricsRepository
    {           
        private static readonly string DatabaseId = ConfigurationManager.AppSettings["database"];
        private static DocumentClient _client;
        private readonly string _collectionName;

        public CosmosDBEfficiencyMetricsRepository(IDataCollectionManager dataCollectionManager)
        {
            _client = new DocumentClient(new Uri(ConfigurationManager.AppSettings["endpoint"]),
                ConfigurationManager.AppSettings["authKey"],
                new ConnectionPolicy
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp
                });

            _collectionName = dataCollectionManager.GetLatestActiveCollectionByDataGroup(DataGroups.EfficiencyMetrics);
        }

        public List<BestInClassDataObject> GetBestInClassDataObjectsByUrn(int urn)
        {        
            var query = $"SELECT * FROM c WHERE c.{EfficiencyMetricDBFieldNames.URN} = @URN";

            SqlQuerySpec querySpec = new SqlQuerySpec(query);
            querySpec.Parameters = new SqlParameterCollection();
            querySpec.Parameters.Add(new SqlParameter("@URN", urn));

            List<BestInClassDataObject> result;
            try
            {
                result = _client.CreateDocumentQuery<BestInClassDataObject>(UriFactory.CreateDocumentCollectionUri(DatabaseId, _collectionName), querySpec, new FeedOptions() { MaxItemCount = 1 }).ToList();                
            }
            catch (Exception ex)
            {
                var errorMessage = $"{_collectionName} could not be loaded! : {ex.Message} : {querySpec.Parameters[0].Name} = {querySpec.Parameters[0].Value}";
                base.LogException(ex, errorMessage);
                return null;
            }
            return result;
        }

        public BestInClassDataObject GetBestInClassDataObjectByURNAndKeyStage(int urn, int keyStage)
        {
            var query = $"SELECT * FROM c WHERE c.{EfficiencyMetricDBFieldNames.URN} = @URN AND c[\"{EfficiencyMetricDBFieldNames.EMC_STAGE}\"]=@EMC_STAGE";

            SqlQuerySpec querySpec = new SqlQuerySpec(query);
            querySpec.Parameters = new SqlParameterCollection();
            querySpec.Parameters.Add(new SqlParameter("@URN", urn));
            querySpec.Parameters.Add(new SqlParameter("@EMC_STAGE", keyStage));

            BestInClassDataObject result;
            try
            {
                result = _client.CreateDocumentQuery<BestInClassDataObject>(UriFactory.CreateDocumentCollectionUri(DatabaseId, _collectionName), querySpec, new FeedOptions() { MaxItemCount = 1 }).ToList().FirstOrDefault();
            }
            catch (Exception ex)
            {
                var errorMessage = $"{_collectionName} could not be loaded! : {ex.Message} : {querySpec.Parameters[0].Name} = {querySpec.Parameters[0].Value}";
                base.LogException(ex, errorMessage);
                return null;
            }
            return result;
        }
    }
}
