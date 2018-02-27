using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    /// <summary>
    ///     Represents a service that can determine whether a feature is available to an operation based on the most recent
    ///     agreement signed by a legal representative of the account.
    /// </summary>
    public interface IFeatureAgreementService
    {
        /// <summary>
        ///     Will deterine if the action is linked to a feature. If the operation is linked to a feature 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<bool> IsFeatureEnabled(OperationContext context);

    }
}