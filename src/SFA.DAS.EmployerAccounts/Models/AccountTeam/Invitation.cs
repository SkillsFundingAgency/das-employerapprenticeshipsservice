using System;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerAccounts.Models.AccountTeam
{
    public class Invitation
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime ExpiryDate { get; set; }
        public InvitationStatus Status { get; set; }
        public Role Role { get; set; }
    }
}