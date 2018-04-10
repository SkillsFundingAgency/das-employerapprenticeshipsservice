using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Authorization;

namespace SFA.DAS.EAS.Infrastructure.Pipeline
{
    public interface IAuthorizationHandler
    {
        Task<bool> CanAccessAsync(IAuthorizationContext authorizationContext);
    }
}
