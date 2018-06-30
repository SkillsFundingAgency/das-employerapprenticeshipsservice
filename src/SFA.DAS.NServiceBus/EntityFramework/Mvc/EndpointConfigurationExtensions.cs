using System.Web.Mvc;
using NServiceBus;

namespace SFA.DAS.NServiceBus.EntityFramework.Mvc
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration SetupEntityFrameworkBehavior(this EndpointConfiguration config, GlobalFilterCollection filters)
        {
            config.SetupEntityFrameworkBehavior();

            filters.Add(new UnitOfWorkManagerFilter(() => DependencyResolver.Current.GetService<IUnitOfWorkManager>()), -999);

            return config;
        }
    }
}