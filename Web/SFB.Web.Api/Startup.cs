using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFB.Web.ApplicationCore.DataAccess;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.Infrastructure.Helpers;
using SFB.Web.Infrastructure.Repositories;
using System.Runtime.Caching;

namespace SFB.Web.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string endPoint = Configuration.GetValue<string>("Values:endpoint");
            string authKey = Configuration.GetValue<string>("Values:authkey");
            string databaseId = Configuration.GetValue<string>("Values:database");

            var cosmosClient = new CosmosClientBuilder(endPoint, authKey)
                                .WithConnectionModeDirect()
                                .Build();

            var dataCollectionManager = new DataCollectionManager(cosmosClient, databaseId, new NetCoreCachedActiveCollectionsService());

            services.AddSingleton<IEfficiencyMetricDataService, EfficiencyMetricDataService>();
            services.AddSingleton<IContextDataService, ContextDataService>();
            services.AddSingleton<IFinancialDataService, FinancialDataService>();
            services.AddSingleton<IFinancialDataRepository>(sp => new CosmosDbFinancialDataRepository(dataCollectionManager, cosmosClient, databaseId));
            services.AddSingleton<IEdubaseRepository>(sp => new CosmosDbEdubaseRepository(dataCollectionManager, cosmosClient, databaseId));
            services.AddSingleton<IEfficiencyMetricRepository>(sp => new EfficiencyMetricRepository(cosmosClient, databaseId));
            services.AddSingleton<IDataCollectionManager>(dataCollectionManager);
            
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
