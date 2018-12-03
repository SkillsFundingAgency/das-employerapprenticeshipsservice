﻿using SFA.DAS.Configuration;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.ReferenceData.Api.Client;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class ReferenceDataRegistry : Registry
    {
        public ReferenceDataRegistry()
        {
            For<IReferenceDataApiConfiguration>().Use(c => c.GetInstance<ReferenceDataApiClientConfiguration>());
            For<ReferenceDataApiClientConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<ReferenceDataApiClientConfiguration>("SFA.DAS.ReferenceDataApiClient")).Singleton();
            For<IReferenceDataService>().Use<ReferenceDataService>().Singleton();
        }
    }
}