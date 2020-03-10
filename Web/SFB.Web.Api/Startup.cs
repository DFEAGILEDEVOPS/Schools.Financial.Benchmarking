using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFB.Web.ApplicationCore.DataAccess;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.Infrastructure.Helpers;
using SFB.Web.Infrastructure.Repositories;

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

            var dataCollectionManager = new DataCollectionManager(cosmosClient, databaseId);

            services.AddSingleton<IEfficiencyMetricDataService, EfficiencyMetricDataService>();
            services.AddSingleton<IContextDataService, ContextDataService>();
            services.AddSingleton<IFinancialDataService, FinancialDataService>();
            services.AddSingleton<IFinancialDataRepository>(new CosmosDbFinancialDataRepository(dataCollectionManager, cosmosClient, databaseId));
            services.AddSingleton<IEdubaseRepository>(new NewCosmosDbEdubaseRepository(dataCollectionManager, cosmosClient, databaseId));
            services.AddSingleton<IEfficiencyMetricRepository>(new EfficiencyMetricRepository(cosmosClient, databaseId));
            services.AddSingleton<IDataCollectionManager>(new DataCollectionManager(cosmosClient, databaseId));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
