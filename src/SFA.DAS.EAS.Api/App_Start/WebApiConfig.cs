using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using SFA.DAS.Authorization.DependencyResolution.StructureMap;
using SFA.DAS.Authorization.WebApi.Extensions;
using SFA.DAS.EAS.Account.Api.App_Start;
using SFA.DAS.EAS.Account.Api.DependencyResolution;
using SFA.DAS.EAS.Account.Api.ExceptionLoggers;
using SFA.DAS.EAS.Application.DependencyResolution;
using Swagger.Net.Application;
using WebApi.StructureMap;

namespace SFA.DAS.EAS.Account.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "EAS Account API");
                    c.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
                })
                .EnableSwaggerUi();

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.MapHttpAttributeRoutes();
            config.Services.Add(typeof(IExceptionLogger), new ErrorLogger());
            config.Services.UseAuthorizationModelBinder();
            config.Filters.AddAuthorizationFilter();
            config.Filters.AddUnauthorizedAccessExceptionFilter();


            config.UseStructureMap(c =>
            {
                c.AddRegistry<AuditRegistry>();
                c.AddRegistry<AuthorizationRegistry>();
                c.AddRegistry<CachesRegistry>();
                c.AddRegistry<CommitmentsRegistry>();
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<EventsRegistry>();
                c.AddRegistry<HashingRegistry>();
                c.AddRegistry<LoggerRegistry>();
                c.AddRegistry<MapperRegistry>();
                c.AddRegistry<MediatorRegistry>();
                c.AddRegistry<MessagePublisherRegistry>();
                c.AddRegistry<NotificationsRegistry>();
                c.AddRegistry<ReferenceDataRegistry>();
                c.AddRegistry<RepositoriesRegistry>();
                c.AddRegistry<ValidationRegistry>();
                c.AddRegistry<EmployerAccountsApiServiceRegistry>();
                c.AddRegistry<EmployerFinanceApiServiceRegistry>();
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}
