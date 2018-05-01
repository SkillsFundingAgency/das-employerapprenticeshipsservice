using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.Features;

namespace SFA.DAS.EAS.Infrastructure.Authorization
{
    public interface IAuthorizationHandler
    {
        Task<AuthorizationResult> CanAccessAsync(IAuthorizationContext authorizationContext, Feature feature);
    }
}