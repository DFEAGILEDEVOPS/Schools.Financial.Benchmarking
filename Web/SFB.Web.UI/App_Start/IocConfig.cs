using System.Configuration;
using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using SFB.Web.ApplicationCore.Services;
using SFB.Web.UI.Helpers;
using SFB.Web.ApplicationCore.Services.DataAccess;
using SFB.Web.ApplicationCore.Services.Search;
using SFB.Web.UI.Services;
using SFB.Web.Infrastructure.Helpers;
using SFB.Web.Infrastructure.Repositories;
using SFB.Web.ApplicationCore.Services.Comparison;
using SFB.Web.ApplicationCore.DataAccess;
using SFB.Web.Infrastructure.Email;
using SFB.Web.Infrastructure.Caching;
using SFB.Web.ApplicationCore.Services.LocalAuthorities;
using SFB.Web.Infrastructure.Logging;
using SFB.Web.Infrastructure.SearchEngine;

namespace SFB.Web.UI
{
    public static class IocConfig
    {
        public static void Register() 
        {
            var builder = new ContainerBuilder();

            // Register your MVC controllers.
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // OPTIONAL: Register model binders that require DI.
            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
            builder.RegisterModelBinderProvider();

            // OPTIONAL: Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            // OPTIONAL: Enable property injection in view pages.
            builder.RegisterSource(new ViewRegistrationSource());

            // OPTIONAL: Enable property injection into action filters.
            builder.RegisterFilterProvider();

            RegisterTypes(builder);

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        private static void RegisterTypes(ContainerBuilder builder)
        {
            RegisterServices(builder);
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<LaSearchService>().As<ILaSearchService>();
            builder.RegisterType<AzureMapsLocationSearchService>().As<ILocationSearchService>();
            builder.RegisterType<BenchmarkChartBuilder>().As<IBenchmarkChartBuilder>();
            builder.RegisterType<HistoricalChartBuilder>().As<IHistoricalChartBuilder>();
            builder.RegisterType<FilterBuilder>().As<IFilterBuilder>();
            builder.RegisterType<ValidationService>().As<IValidationService>();
            builder.RegisterType<FinancialDataService>().As<IFinancialDataService>().SingleInstance();
            builder.RegisterType<FinancialDataService>().As<ITermYearDataService>().SingleInstance();
            builder.RegisterType<EfficiencyMetricDataService>().As<IEfficiencyMetricDataService>().SingleInstance();
            builder.RegisterType<ContextDataService>().As<IContextDataService>().SingleInstance();
            builder.RegisterType<DataCollectionManager>().As<IDataCollectionManager>().SingleInstance();
            builder.RegisterType<CosmosDbEdubaseRepository>().As<IEdubaseRepository>().SingleInstance();
            builder.RegisterType<CosmosDbFinancialDataRepository>().As<IFinancialDataRepository>().SingleInstance();
            builder.RegisterType<CosmosDBEfficiencyMetricRepository>().As<IEfficiencyMetricRepository>().SingleInstance();
            builder.RegisterType<CosmosDbTrustHistoryRepository>().As<ITrustHistoryRepository>().SingleInstance();
            builder.RegisterType<FinancialCalculationsService>().As<IFinancialCalculationsService>();
            builder.RegisterType<SchoolBenchmarkListService>().As<ISchoolBenchmarkListService>();
            builder.RegisterType<ManualBenchmarkListService>().As<IManualBenchmarkListService>();
            builder.RegisterType<TrustBenchmarkListService>().As<ITrustBenchmarkListService>();
            builder.RegisterType<ComparisonService>().As<IComparisonService>();
            builder.RegisterType<SchoolVMWithHistoricalChartsBuilder>().As<ISchoolVMBuilder>();
            builder.RegisterType<SchoolBenchmarkListService>().As<ISchoolBenchmarkListService>();
            builder.RegisterType<BenchmarkCriteriaBuilderService>().As<IBenchmarkCriteriaBuilderService>();
            builder.RegisterType<DownloadCSVBuilder>().As<IDownloadCSVBuilder>();
            builder.RegisterType<NotifyEmailSendingService>().As<IEmailSendingService>();
            builder.RegisterType<AspNetCachedLocalAuthoritiesService>().As<ILocalAuthoritiesService>();
            builder.RegisterType<RedisCachedActiveUrnsService>().As<IActiveUrnsService>().SingleInstance();
            builder.RegisterType<AspNetCachedActiveCollectionsService>().As<IActiveCollectionsService>().SingleInstance();
            builder.RegisterType<TrustHistoryService>().As<ITrustHistoryService>().SingleInstance();
            builder.RegisterType<RedisCachedBicComparisonResultCachingService>().As<IBicComparisonResultCachingService>().SingleInstance();
            //builder.RegisterInstance(new RedDogSchoolSearchService(ConfigurationManager.AppSettings["SearchInstance"], ConfigurationManager.AppSettings["SearchKey"], ConfigurationManager.AppSettings["SearchIndex"])).As<ISchoolSearchService>();            
            builder.RegisterInstance(new AzureSchoolSearchService(ConfigurationManager.AppSettings["SearchInstance"], ConfigurationManager.AppSettings["SearchKey"], ConfigurationManager.AppSettings["SearchIndex"])).As<ISchoolSearchService>();            
            builder.RegisterInstance(new AzureTrustSearchService(ConfigurationManager.AppSettings["SearchInstance"], ConfigurationManager.AppSettings["SearchKey"], ConfigurationManager.AppSettings["SearchIndexTrust"])).As<ITrustSearchService>();
            builder.RegisterInstance(new AspNetLogManager(ConfigurationManager.AppSettings["EnableAITelemetry"])).As<ILogManager>();
        }
    }
}