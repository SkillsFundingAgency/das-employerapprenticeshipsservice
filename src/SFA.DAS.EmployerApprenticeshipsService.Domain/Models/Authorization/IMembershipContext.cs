using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Domain.Models.Authorization
{
    public interface IMembershipContext
    {
        Role Role { get; }
    }
}