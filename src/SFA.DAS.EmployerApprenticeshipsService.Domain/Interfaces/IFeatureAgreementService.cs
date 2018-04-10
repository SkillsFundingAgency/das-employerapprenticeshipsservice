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
        ///     Will deterine if the action is linked to a feature controlled by an agreement and if it 
        ///     is determime if the current account has signed atleast that agreement. 
        /// </summary>
        Task<bool> IsFeatureEnabled(OperationContext context);

    }
}