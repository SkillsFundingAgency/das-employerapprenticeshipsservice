using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.Services.Features
{
    /// <summary>
    ///     Represents a service that can determine whether a feature is whitelisted and whether the current user
    ///     is included in the whitelist.
    /// </summary>
    public interface IFeatureWhiteListingService
    {
        /// <summary>
        ///     Checks the operation context against the configured feature toggles to determine if the operation
        ///     should have access.
        /// </summary>
        /// <returns>
        ///     true if the operation is either not subject to a feature toggle or the operaton is feature toggled but the user in the <see cref="OperationContext"/> 
        ///     is whbite listed.
        ///     false if the operation is feature toggled and the current user is not white listed.
        /// </returns>
        Task<bool> IsFeatureEnabledForContextAsync(OperationContext context);
    }
}