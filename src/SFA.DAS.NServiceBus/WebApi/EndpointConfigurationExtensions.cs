using System.Web.Http.Filters;
using NServiceBus;

namespace SFA.DAS.NServiceBus.WebApi
{
    public static class EndpointConfigurationExtensions
    {
        internal static EndpointConfiguration SetupUnitOfWork(this EndpointConfiguration config, HttpFilterCollection filters)
        {
            config.SetupUnitOfWork();
            filters.Add(new UnitOfWorkManagerFilter());

            return config;
        }
    }
}