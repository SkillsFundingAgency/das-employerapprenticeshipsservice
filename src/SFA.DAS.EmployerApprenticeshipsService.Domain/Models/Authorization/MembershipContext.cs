using System;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Domain.Models.Authorization
{
    public class MembershipContext : IMembershipContext
    {
        public string AccountHashedId { get; set; }
        public long AccountId { get; set; }
        public Guid UserExternalId { get; set; }
        public long UserId { get; set; }
        public string UserEmail { get; set; }
        public Role Role { get; set; }
    }
}