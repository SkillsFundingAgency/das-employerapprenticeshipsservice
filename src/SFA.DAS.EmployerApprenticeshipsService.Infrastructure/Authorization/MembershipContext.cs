using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Infrastructure.Authorization
{
    public class MembershipContext : IMembershipContext
    {
        public Role Role { get; set; }
    }
}