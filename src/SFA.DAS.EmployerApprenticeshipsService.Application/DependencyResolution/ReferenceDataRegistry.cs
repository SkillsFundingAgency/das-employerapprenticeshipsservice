using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
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
        }
    }
}