using System.Data.Entity;
using System.Web.Http.Filters;
using NServiceBus;

namespace SFA.DAS.NServiceBus.EntityFramework.WebApi
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration SetupEntityFrameworkBehavior<T>(this EndpointConfiguration config, HttpFilterCollection filters) where T : DbContext, IOutboxDbContext
        {
            config.SetupEntityFrameworkBehavior<T>();

            filters.Add(new UnitOfWorkManagerFilter());

            return config;
        }
    }
}