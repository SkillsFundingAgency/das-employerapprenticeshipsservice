using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Web.Authentication
{
    public interface ICurrentUserService
    {
        CurrentUser GetCurrentUser();
    }
}