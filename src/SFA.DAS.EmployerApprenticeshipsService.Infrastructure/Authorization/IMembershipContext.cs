using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Infrastructure.Authorization
{
    public interface IMembershipContext
    {
        Role Role { get; }
    }
}