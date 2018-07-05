using System.Web.Mvc;
using NServiceBus;

namespace SFA.DAS.NServiceBus.Mvc
{
    public static class EndpointConfigurationExtensions
    {
        internal static EndpointConfiguration SetupUnitOfWork(this EndpointConfiguration config, GlobalFilterCollection filters)
        {
            config.SetupUnitOfWork();
            filters.Add(new UnitOfWorkManagerFilter(() => DependencyResolver.Current.GetService<IUnitOfWorkManager>()), -999);

            return config;
        }
    }
}