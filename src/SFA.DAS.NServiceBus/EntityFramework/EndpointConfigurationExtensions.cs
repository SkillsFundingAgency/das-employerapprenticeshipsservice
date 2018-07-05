using System.Data.Entity;
using System.Web.Http.Filters;
using System.Web.Mvc;
using NServiceBus;
using SFA.DAS.NServiceBus.Mvc;
using SFA.DAS.NServiceBus.WebApi;

namespace SFA.DAS.NServiceBus.EntityFramework
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration SetupEntityFrameworkUnitOfWork<T>(this EndpointConfiguration config) where T : DbContext
        {
            config.RegisterComponents(c =>
            {
                c.ConfigureComponent<Db<T>>(DependencyLifecycle.InstancePerUnitOfWork);
            });

            config.SetupUnitOfWork();

            return config;
        }

        public static EndpointConfiguration SetupEntityFrameworkUnitOfWork<T>(this EndpointConfiguration config, GlobalFilterCollection filters) where T : DbContext, IOutboxDbContext
        {
            config.RegisterComponents(c =>
            {
                c.ConfigureComponent<Db<T>>(DependencyLifecycle.InstancePerUnitOfWork);
                c.ConfigureComponent<Outbox<T>>(DependencyLifecycle.InstancePerUnitOfWork);
            });
            
            config.SetupUnitOfWork(filters);

            return config;
        }

        public static EndpointConfiguration SetupEntityFrameworkUnitOfWork<T>(this EndpointConfiguration config, HttpFilterCollection filters) where T : DbContext, IOutboxDbContext
        {
            config.RegisterComponents(c =>
            {
                c.ConfigureComponent<Db<T>>(DependencyLifecycle.InstancePerUnitOfWork);
                c.ConfigureComponent<Outbox<T>>(DependencyLifecycle.InstancePerUnitOfWork);
            });
            
            config.SetupUnitOfWork(filters);

            return config;
        }
    }
}