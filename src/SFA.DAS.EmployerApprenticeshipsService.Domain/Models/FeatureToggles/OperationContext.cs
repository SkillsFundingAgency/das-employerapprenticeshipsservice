using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Authorization;

namespace SFA.DAS.EAS.Domain.Models.FeatureToggles
{
    public static class FeatureHandlerResults
    {
        public static readonly Task<bool> FeatureEnabledTask = Task.FromResult(true);
        public static readonly Task<bool> FeatureDisabledTask = Task.FromResult(false);
    }

}
