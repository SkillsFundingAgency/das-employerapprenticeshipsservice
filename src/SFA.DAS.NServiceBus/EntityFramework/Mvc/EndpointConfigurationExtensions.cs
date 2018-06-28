using System.Data.Entity;
using System.Web.Mvc;
using NServiceBus;

namespace SFA.DAS.NServiceBus.EntityFramework.Mvc
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration SetupEntityFrameworkBehavior<T>(this EndpointConfiguration config, GlobalFilterCollection filters) where T : DbContext, IOutboxDbContext
        {
            config.SetupEntityFrameworkBehavior<T>();

            filters.Add(new UnitOfWorkManagerFilter(() => DependencyResolver.Current.GetService<IUnitOfWorkManager>()), -999);

            return config;
        }
    }
}