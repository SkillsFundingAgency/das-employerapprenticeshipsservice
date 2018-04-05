using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.Pipeline
{

    public interface IOperationAuthorisationHandler
    {
        Task<bool> CanAccessAsync(IAuthorizationContext context);
    }
}
