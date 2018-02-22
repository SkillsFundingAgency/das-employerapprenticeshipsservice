using System;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Domain.Models.Authorization
{
    public class MembershipContext : IMembershipContext
    {
        public long AccountId { get; set; }
        public string AccountHashedId { get; set; }
        public string AccountPublicHashedId { get; set; }
        public long UserId { get; set; }
        public Guid UserExternalId { get; set; }
        public string UserEmail { get; set; }
        public Role Role { get; set; }
    }
}