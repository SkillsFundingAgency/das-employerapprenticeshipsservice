using SFA.DAS.EAS.Portal.Client.Services.DasRecruit;
using StructureMap;

namespace SFA.DAS.EAS.Portal.Client.DependencyResolution.StructureMap
{
    public class RecruitApiRegistry : Registry
    {
        public RecruitApiRegistry()
        {
            //For<RecruitApiHttpClientFactory>().Use<RecruitApiHttpClientFactory>();
            For<IDasRecruitService>().Use<DasRecruitService>();
        }
    }
}