using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.ReferenceData.Api.Client;

namespace SFA.DAS.EmployerAccounts.DependencyResolution;

public class ReferenceDataRegistry : Registry
{
    public ReferenceDataRegistry()
    {
        For<IReferenceDataApiConfiguration>().Use(c => c.GetInstance<ReferenceDataApiClientConfiguration>());
        For<IReferenceDataService>().Use<ReferenceDataService>().Singleton();
    }
}