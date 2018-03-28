using System.Configuration;
using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using SFB.Web.UI.Utils;
using SFB.Web.Domain;
using SFB.Web.Domain.Services;
using SFB.Web.UI.Helpers;
using System.Web.Configuration;
using SFB.Web.Domain.ApiWrappers;
using SFB.Web.Domain.Services.DataAccess;
using SFB.Web.Domain.Services.Search;
using SFB.Web.UI.Services;
using SFB.Web.DAL.Helpers;
using SFB.Web.DAL.Repositories;

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
            RegisterWrappers(builder);
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<LocalAuthoritiesService>().As<ILocalAuthoritiesService>();
            builder.RegisterType<BenchmarkChartBuilder>().As<IBenchmarkChartBuilder>();
            builder.RegisterType<HistoricalChartBuilder>().As<IHistoricalChartBuilder>();
            builder.RegisterType<FilterBuilder>().As<IFilterBuilder>();
            builder.RegisterType<ValidationService>().As<IValidationService>();
            builder.RegisterType<FinancialDataService>().As<IFinancialDataService>().SingleInstance();
            builder.RegisterType<FinancialDataService>().As<ITermYearDataService>().SingleInstance();
            builder.RegisterType<ContextDataService>().As<IContextDataService>().SingleInstance();
            builder.RegisterType<DataCollectionManager>().As<IDataCollectionManager>().SingleInstance();
            builder.RegisterType<DocumentDbEdubaseRepository>().As<IEdubaseRepository>().SingleInstance();
            builder.RegisterType<DocumentDbFinancialDataRepository>().As<IFinancialDataRepository>().SingleInstance();
            builder.RegisterType<FinancialCalculationsService>().As<IFinancialCalculationsService>();
            builder.RegisterType<StatisticalCriteriaBuilderService>().As<IStatisticalCriteriaBuilderService>();
            builder.RegisterType<DownloadCSVBuilder>().As<IDownloadCSVBuilder>();
            builder.RegisterInstance(new SchoolSearchService(ConfigurationManager.AppSettings["SearchInstance"],ConfigurationManager.AppSettings["SearchKey"], ConfigurationManager.AppSettings["SearchIndex"], ConfigurationManager.AppSettings["GoogleAPIKey"])).As<ISchoolSearchService>();
            builder.RegisterInstance(new TrustSearchService(ConfigurationManager.AppSettings["SearchInstance"],ConfigurationManager.AppSettings["SearchKey"], ConfigurationManager.AppSettings["SearchIndexTrust"])).As<ITrustSearchService>();
        }

        private static void RegisterWrappers(ContainerBuilder builder)
        {
           builder.RegisterType<RequestContextWrapper>().As<IRequestContext>();
           builder.Register(c => new ApiRequest(WebConfigurationManager.AppSettings["DfeApiUrl"], WebConfigurationManager.AppSettings["DfEApiUserName"], WebConfigurationManager.AppSettings["DfEApiPassword"])).As<IApiRequest>();
        }
    }
}