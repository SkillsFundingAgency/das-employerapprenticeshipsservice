using SFA.DAS.EAS.Portal.Client.Http;
using SFA.DAS.EAS.Portal.Client.Services.DasRecruit;
using StructureMap;

namespace SFA.DAS.EAS.Portal.Client.DependencyResolution.StructureMap
{
    public class RecruitApiRegistry : Registry
    {
        public RecruitApiRegistry()
        {
            For<IRecruitApiHttpClientFactory>().Use<RecruitApiHttpClientFactory>();
            For<IDasRecruitService>().Use<DasRecruitService>().Singleton();
        }
    }
}