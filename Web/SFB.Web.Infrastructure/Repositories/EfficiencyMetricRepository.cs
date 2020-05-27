using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using SFB.Web.ApplicationCore.DataAccess;
using SFB.Web.ApplicationCore.Entities;
using SFB.Web.Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFB.Web.Infrastructure.Repositories
{
    public class EfficiencyMetricRepository : AppInsightsLoggable, IEfficiencyMetricRepository
    {
        private readonly string _databaseId;
        private static CosmosClient _client;

        public EfficiencyMetricRepository(ILogManager logManager) : base(logManager)
        {
            var clientBuilder = new CosmosClientBuilder(ConfigurationManager.AppSettings["endpoint"], ConfigurationManager.AppSettings["authKey"]);

            _client = clientBuilder.WithConnectionModeDirect().Build();

            _databaseId = _databaseId = ConfigurationManager.AppSettings["database"];
        }

        public EfficiencyMetricRepository(CosmosClient cosmosClient, string databaseId, ILogManager logManager) : base(logManager)
        {
            _client = cosmosClient;
            _databaseId = databaseId;
        }

        public async Task<EfficiencyMetricDataObject> GetEfficiencyMetricDataObjectByUrnAsync(int urn)
        {
            var container = _client.GetContainer(_databaseId, "EMPoc");

            var queryString = $"SELECT * FROM c WHERE c.urn=@URN";

            var queryDefinition = new QueryDefinition(queryString)
                .WithParameter($"@URN", urn);

            var feedIterator = container.GetItemQueryIterator<EfficiencyMetricDataObject>(queryDefinition, null);
            var result = (await feedIterator.ReadNextAsync()).FirstOrDefault();

            return result;
        }

        public async Task<List<EfficiencyMetricDataObject>> GetMultipleSchoolDataObjectsByUrnsAsync(List<int> urns)
        {
            var container = _client.GetContainer(_databaseId, "EMPoc");

            var sb = new StringBuilder();
            urns.ForEach(u => sb.Append(u + ","));

            var queryString = $"SELECT * FROM c WHERE c.urn in ({sb.ToString().TrimEnd(',')})";

            var results = new List<EfficiencyMetricDataObject>();
            try
            {
                var query = container.GetItemQueryIterator<EfficiencyMetricDataObject>(new QueryDefinition(queryString));
                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();

                    results.AddRange(response.ToList());
                }
                if (results.Count < urns.Count)
                {
                    throw new Newtonsoft.Json.JsonSerializationException();
                }
                return results;
            }
            catch (Exception ex)
            {
                var errorMessage = $"EM collection could not be loaded! : {ex.Message} : URNs = {sb.ToString().TrimEnd(',')}";
                base.LogException(ex, errorMessage);
                throw new ApplicationException($"One or more documents could not be loaded from EM collection : URNs = {sb.ToString().TrimEnd(',')}");
            }

        }
    }
}
