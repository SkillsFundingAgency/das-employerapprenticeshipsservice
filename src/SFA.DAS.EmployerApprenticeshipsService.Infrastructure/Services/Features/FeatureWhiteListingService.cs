using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.Services.Features
{
    public class FeatureWhiteListingService : IFeatureWhiteListingService
    {
        private readonly IFeatureService _featureService;

        public FeatureWhiteListingService(IFeatureService featureService)
        {
            _featureService = featureService;
        }

        public async Task<bool> IsFeatureEnabledForContextAsync(OperationContext context)
        {
            var whitelist = await _featureService.GetWhitelistForOperationAsync(context);
            if (whitelist == null)
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(context.MembershipContext?.UserEmail))
                return false;

            return whitelist.Any(email =>
                Regex.IsMatch(context.MembershipContext.UserEmail, email, RegexOptions.IgnoreCase));
        }
    }
}