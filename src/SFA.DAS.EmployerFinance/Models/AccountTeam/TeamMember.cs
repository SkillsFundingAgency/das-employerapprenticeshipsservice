using System;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerFinance.Models.AccountTeam
{
    public class TeamMember
    {
        public bool IsUser { get; set; }
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string HashedAccountId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string UserRef { get; set; }
        public Role Role { get; set; }
        public InvitationStatus Status { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string HashedInvitationId { get; set; }
        public bool CanReceiveNotifications { get; set; }
    }
}
