using System.Web.Http.Filters;
using NServiceBus;

namespace SFA.DAS.NServiceBus.EntityFramework.WebApi
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration SetupEntityFrameworkBehavior(this EndpointConfiguration config, HttpFilterCollection filters)
        {
            config.SetupEntityFrameworkBehavior();

            filters.Add(new UnitOfWorkManagerFilter());

            return config;
        }
    }
}