using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    /// <summary>
    ///     Represents a service that can determine whether a feature is toggled if so whether it should be available 
    ///     to the current <see cref="OperationContext"/>.
    /// </summary>
    public interface IFeatureToggleService
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
        Task<bool> IsFeatureEnabled(OperationContext context);
    }
}