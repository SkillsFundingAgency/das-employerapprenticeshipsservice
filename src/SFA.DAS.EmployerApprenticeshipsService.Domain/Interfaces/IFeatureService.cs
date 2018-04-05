using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    /// <summary>
    ///     Provides access to feature and associated controller actions.
    /// </summary>
    public interface IFeatureService
    {
        Task<Feature> GetFeatureThatAllowsAccessToOperationAsync(string controller, string action);
    }
}