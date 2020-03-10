using Microsoft.Azure.Cosmos;
using SFB.Web.ApplicationCore.DataAccess;
using SFB.Web.ApplicationCore.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace SFB.Web.Infrastructure.Repositories
{
    public class EfficiencyMetricRepository : IEfficiencyMetricRepository
    {
        private readonly string _databaseId;
        private static CosmosClient _client;

        public EfficiencyMetricRepository(CosmosClient cosmosClient, string databaseId)
        {
            _client = cosmosClient;
            _databaseId = databaseId;
        }

        public async Task<EfficiencyMetricDataObject> GetEfficiencyMetricDataObjectByUrnAsync(int urn)
        {
            var container = _client.GetContainer(_databaseId, "EfficienctMetricSecondaryPOC");

            var queryString = $"SELECT * FROM c WHERE c.urn=@URN";

            var queryDefinition = new QueryDefinition(queryString)
                .WithParameter($"@URN", urn);

            var feedIterator = container.GetItemQueryIterator<EfficiencyMetricDataObject>(queryDefinition, null);
            var result = (await feedIterator.ReadNextAsync()).FirstOrDefault();

            return result;
        }
    }
}
