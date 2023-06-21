using SFA.DAS.EAS.Support.Infrastructure.Models;

namespace SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

public interface IChallengeRepository
{
    Task<bool> CheckData(Core.Models.Account record, ChallengePermissionQuery message);
}