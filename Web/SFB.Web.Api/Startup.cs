using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFB.Web.ApplicationCore.DataAccess;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.Infrastructure.Helpers;
using SFB.Web.Infrastructure.Logging;
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
            string endPoint = Configuration.GetValue<string>("Secrets:endpoint");
            string authKey = Configuration.GetValue<string>("Secrets:authkey");
            string databaseId = Configuration.GetValue<string>("Secrets:database");
            string enableAiTelemetry = Configuration.GetValue<string>("ApplicationInsights:enabled");

            var cosmosClient = new CosmosClientBuilder(endPoint, authKey)
                                .WithConnectionModeDirect()
                                .Build();

            var dataCollectionManager = new DataCollectionManager(cosmosClient, databaseId, new NetCoreCachedActiveCollectionsService());

            services.AddSingleton<ILogManager>(container => new NetCoreLogManager(container.GetRequiredService<IHttpContextAccessor>(), enableAiTelemetry));
            services.AddSingleton<IEfficiencyMetricDataService, EfficiencyMetricDataService>();
            services.AddSingleton<IContextDataService, ContextDataService>();
            //services.AddSingleton<IFinancialDataService, FinancialDataService>();
            //services.AddSingleton<IFinancialDataRepository>(container => new CosmosDbFinancialDataRepository(dataCollectionManager, cosmosClient, databaseId, container.GetRequiredService<ILogManager>()));
            services.AddSingleton<IEdubaseRepository>(container => new CosmosDbEdubaseRepository(dataCollectionManager, cosmosClient, databaseId, container.GetRequiredService<ILogManager>()));
            services.AddSingleton<IEfficiencyMetricRepository>(container => new EfficiencyMetricRepository(cosmosClient, databaseId, container.GetRequiredService<ILogManager>()));
            services.AddSingleton<IDataCollectionManager>(dataCollectionManager);

            services.AddApplicationInsightsTelemetry();
            
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:4200");
                    });
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
