using SFA.DAS.AutoConfiguration;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.ReferenceData.Api.Client;
using SFA.DAS.Tasks.API.Client;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class ReferenceDataRegistry : Registry
    {
        public ReferenceDataRegistry()
        {
            For<IReferenceDataApiConfiguration>().Use(c => c.GetInstance<ReferenceDataApiClientConfiguration>());
            For<ITaskApiConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<ITaskApiConfiguration>(ConfigurationKeys.ReferenceDataApiClient)).Singleton();

            For<IReferenceDataService>().Use<ReferenceDataService>().Singleton();
        }
    }
}