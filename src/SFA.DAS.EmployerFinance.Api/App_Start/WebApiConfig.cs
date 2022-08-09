using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using SFA.DAS.Authorization.DependencyResolution.StructureMap;
using SFA.DAS.Authorization.WebApi.Extensions;
using SFA.DAS.EmployerFinance.Api.DependencyResolution;
using SFA.DAS.EmployerFinance.Api.ExceptionLoggers;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.DependencyResolution;
using SFA.DAS.UnitOfWork.EntityFramework.StructureMap;
using SFA.DAS.UnitOfWork.NServiceBus.DependencyResolution.StructureMap;
using SFA.DAS.UnitOfWork.NServiceBus.Features.ClientOutbox.DependencyResolution.StructureMap;
using SFA.DAS.UnitOfWork.WebApi.Extensions;
using SFA.DAS.Validation.WebApi;
using WebApi.StructureMap;
using SFA.DAS.EAS.Application.DependencyResolution;
using SFA.DAS.EmployerFinance.Api.Client;
using SFA.DAS.EmployerFinance.Configuration;

namespace SFA.DAS.EmployerFinance.Api
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
                c.AddRegistry<EmployerFinance.DependencyResolution.CachesRegistry>();                
                c.AddRegistry<EAS.Application.DependencyResolution.ConfigurationRegistry>();
                c.AddRegistry<EmployerFinance.DependencyResolution.DataRegistry>();
                c.AddRegistry<DateTimeRegistry>();
                c.AddRegistry<EntityFrameworkUnitOfWorkRegistry<EmployerFinanceDbContext>>();
                c.AddRegistry<EmployerFinance.DependencyResolution.EventsRegistry>();
                c.AddRegistry<ExecutionPoliciesRegistry>();
                c.AddRegistry<EmployerFinance.DependencyResolution.HashingRegistry>();
                c.AddRegistry<EAS.Application.DependencyResolution.LoggerRegistry>();
                c.AddRegistry<EmployerFinance.DependencyResolution.MapperRegistry>();    
                c.AddRegistry<EmployerFinance.DependencyResolution.MediatorRegistry>();
                c.AddRegistry<EmployerFinance.DependencyResolution.MessagePublisherRegistry>();
                c.AddRegistry<EmployerFinance.DependencyResolution.NotificationsRegistry>();
                c.AddRegistry<NServiceBusClientUnitOfWorkRegistry>();
                c.AddRegistry<NServiceBusUnitOfWorkRegistry>();
                c.AddRegistry<StartupRegistry>();
                c.AddRegistry<DefaultRegistry>();
                c.AddRegistry<CommitmentsV2ApiClientRegistry>();
                c.AddRegistry(new EmployerFinanceApiClientRegistry(context => context.GetInstance<EmployerFinanceConfiguration>().EmployerFinanceApi));
            });
        }
    }
}
