using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using SFB.Web.Common;
using SFB.Web.Common.DataObjects;
using SFB.Web.DAL.Helpers;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

namespace SFB.Web.DAL.Repositories
{
    public class CosmosDBEfficiencyMetricsRepository : IEfficiencyMetricsRepository
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

        public BestInBreedDataObject GetBestInBreedDataObjectByUrn(int urn)
        {        
            var query = $"SELECT * FROM c WHERE c.{EfficiencyMetricDBFieldNames.URN} = @URN";

            SqlQuerySpec querySpec = new SqlQuerySpec(query);
            querySpec.Parameters = new SqlParameterCollection();
            querySpec.Parameters.Add(new SqlParameter("@URN", urn));

            BestInBreedDataObject result;
            try
            {
                result = _client.CreateDocumentQuery<BestInBreedDataObject>(UriFactory.CreateDocumentCollectionUri(DatabaseId, "20180710012750-EfficiencyMetrics-2016-2017"), querySpec, new FeedOptions() { MaxItemCount = 1 }).ToList().FirstOrDefault();
                //TODO: correct
                //result = _client.CreateDocumentQuery<BestInBreedDataObject>(UriFactory.CreateDocumentCollectionUri(DatabaseId, _collectionName), querySpec, new FeedOptions() { MaxItemCount = 1 }).ToList().FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex is Newtonsoft.Json.JsonSerializationException || ex is Newtonsoft.Json.JsonReaderException)
                {
                    var errorMessage = $"{_collectionName} could not be loaded! : {ex.Message} : {querySpec.Parameters[0].Name} = {querySpec.Parameters[0].Value}";
                    Debug.WriteLine(errorMessage);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new ApplicationException(errorMessage));
                }
                return null;
            }
            return result;
        }
    }
}
