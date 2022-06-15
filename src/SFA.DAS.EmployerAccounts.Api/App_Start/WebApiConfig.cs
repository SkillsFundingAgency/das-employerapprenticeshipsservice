using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using SFA.DAS.Authorization.DependencyResolution.StructureMap;
using SFA.DAS.Authorization.WebApi.Extensions;
using SFA.DAS.EmployerAccounts.Api.DependencyResolution;
using SFA.DAS.EmployerAccounts.Api.ExceptionLoggers;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.DependencyResolution;
using SFA.DAS.UnitOfWork.EntityFramework.StructureMap;
using SFA.DAS.UnitOfWork.NServiceBus.DependencyResolution.StructureMap;
using SFA.DAS.UnitOfWork.NServiceBus.Features.ClientOutbox.DependencyResolution.StructureMap;
using SFA.DAS.UnitOfWork.WebApi.Extensions;
using SFA.DAS.Validation.WebApi;
using WebApi.StructureMap;

namespace SFA.DAS.EmployerAccounts.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Filters.AddUnitOfWorkFilter();
            config.Filters.Add(new ValidateModelStateFilter());
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.MapHttpAttributeRoutes();
            config.Services.Add(typeof(IExceptionLogger), new ErrorLogger());
            config.Services.UseAuthorizationModelBinder();
            config.Filters.AddAuthorizationFilter();
            config.Filters.AddUnauthorizedAccessExceptionFilter();

            config.UseStructureMap(c =>
            {
                c.AddRegistry<AuthorizationRegistry>();
                c.AddRegistry<CachesRegistry>();
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<DataRegistry>();
                c.AddRegistry<EntityFrameworkUnitOfWorkRegistry<EmployerAccountsDbContext>>();
                c.AddRegistry<EventsRegistry>();
                c.AddRegistry<ExecutionPoliciesRegistry>();
                c.AddRegistry<HashingRegistry>();
                c.AddRegistry<LoggerRegistry>();
                c.AddRegistry<MapperRegistry>();
                c.AddRegistry<MediatorRegistry>();
                c.AddRegistry<MessagePublisherRegistry>();
                c.AddRegistry<NotificationsRegistry>();
                c.AddRegistry<NServiceBusClientUnitOfWorkRegistry>();
                c.AddRegistry<NServiceBusUnitOfWorkRegistry>();
                c.AddRegistry<RepositoriesRegistry>();
                c.AddRegistry<TokenServiceRegistry>();
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}
