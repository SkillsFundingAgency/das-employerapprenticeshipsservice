using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class LevyRegistry : Registry
    {
        public LevyRegistry()
        {
            For<LevyDeclarationProviderConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<LevyDeclarationProviderConfiguration>("SFA.DAS.LevyAggregationProvider")).Singleton();
        }
    }
}