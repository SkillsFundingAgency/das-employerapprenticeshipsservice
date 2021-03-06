﻿using System.Net.Http;
using SFA.DAS.EmployerFinance.Infrastructure;
using SFA.DAS.EmployerFinance.Interfaces.OuterApi;
using SFA.DAS.EmployerFinance.Services;
using StructureMap;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class ProvidersRegistry : Registry
    {
        public ProvidersRegistry()
        {
            For<IProviderService>().Use<ProviderServiceFromDb>();
            For<IProviderService>().DecorateAllWith<ProviderServiceRemote>();
            For<IProviderService>().DecorateAllWith<ProviderServiceCache>();
        }
    }
}