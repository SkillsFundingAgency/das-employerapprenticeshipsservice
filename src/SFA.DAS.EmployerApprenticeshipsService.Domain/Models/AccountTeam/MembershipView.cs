using SFA.DAS.Authorization;
using System;

namespace SFA.DAS.EAS.Domain.Models.AccountTeam
{
    public class MembershipView
    {
        public string HashedAccountId { get; set; }
        public long AccountId { get; set; }
        public string AccountName { get; set; }
        public long UserId { get; set; }
        public string UserRef { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool ShowWizard { get; set; }
        public string FullName() => $"{FirstName} {LastName}";
    }
}