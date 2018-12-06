using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.ModelBinding;
using SFA.DAS.Authorization.WebApi;
using SFA.DAS.EAS.Account.Api.DependencyResolution;
using SFA.DAS.EAS.Account.Api.ExceptionLoggers;
using SFA.DAS.EAS.Application.DependencyResolution;
using SFA.DAS.Validation.WebApi;
using WebApi.StructureMap;

namespace SFA.DAS.EAS.Account.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Filters.Add(new ValidateModelStateFilter());
            config.Filters.Add(new HandleErrorFilter());
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.MapHttpAttributeRoutes();
            config.Services.Add(typeof(IExceptionLogger), new ErrorLogger());
            config.Services.Insert(typeof(ModelBinderProvider), 0, new MessageModelBinderProvider());

            config.UseStructureMap(c =>
            {
                c.AddRegistry<AuditRegistry>();
                c.AddRegistry<AuthorizationRegistry>();
                c.AddRegistry<CachesRegistry>();
                c.AddRegistry<CommitmentsRegistry>();
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<DateTimeRegistry>();
                c.AddRegistry<EventsRegistry>();
                c.AddRegistry<ExecutionPoliciesRegistry>();
                c.AddRegistry<HashingRegistry>();
                c.AddRegistry<LoggerRegistry>();
                c.AddRegistry<MapperRegistry>();
                c.AddRegistry<MediatorRegistry>();
                c.AddRegistry<MessagePublisherRegistry>();
                c.AddRegistry<NotificationsRegistry>();
                c.AddRegistry<ReferenceDataRegistry>();
                c.AddRegistry<RepositoriesRegistry>();
                c.AddRegistry<ServicesRegistry>();
                c.AddRegistry<TasksRegistry>();
                c.AddRegistry<TokenServiceRegistry>();
                c.AddRegistry<ValidationRegistry>();
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}
