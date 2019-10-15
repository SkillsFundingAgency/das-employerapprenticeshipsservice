using SFA.DAS.AutoConfiguration;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.ReferenceData.Api.Client;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class ReferenceDataRegistry : Registry
    {
        public ReferenceDataRegistry()
        {
            For<IReferenceDataApiConfiguration>().Use(c => c.GetInstance<ReferenceDataApiClientConfiguration>());
            For<ReferenceDataApiClientConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<ReferenceDataApiClientConfiguration>(ConfigurationKeys.ReferenceDataApi)).Singleton();
        }
    }
}