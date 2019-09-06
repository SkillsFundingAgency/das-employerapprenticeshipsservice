using SFA.DAS.EAS.Portal.Client.Services.Recruit;
using SFA.DAS.EAS.Portal.Client.Services.Recruit.Http;
using StructureMap;

namespace SFA.DAS.EAS.Portal.Client.DependencyResolution.StructureMap
{
    public class RecruitApiRegistry : Registry
    {
        public RecruitApiRegistry()
        {
            For<IRecruitApiHttpClientFactory>().Use<RecruitApiHttpClientFactory>();
            For<IRecruitService>().Use<RecruitService>().Singleton();
        }
    }
}