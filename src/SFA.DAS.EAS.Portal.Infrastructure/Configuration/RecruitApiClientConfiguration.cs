using System.Threading.Tasks;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;

namespace SFA.DAS.EAS.DasRecruitService
{
    public class RecruitApiClientConfiguration: IJwtClientConfiguration
    {
        public string ApiBaseUrl { get; }
        public string ClientToken { get; }
    }
}
