using System.Threading.Tasks;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Infrastructure.Models;

namespace SFA.DAS.EAS.Support.ApplicationServices;

public interface IChallengeHandler
{
    Task<ChallengeResponse> Get(string id);
    Task<ChallengePermissionResponse> Handle(ChallengePermissionQuery message);
}