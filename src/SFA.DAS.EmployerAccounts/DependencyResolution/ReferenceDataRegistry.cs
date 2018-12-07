using SFA.DAS.Configuration;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.ReferenceData.Api.Client;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
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