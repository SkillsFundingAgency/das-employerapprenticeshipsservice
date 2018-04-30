using System;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Domain.Models.AccountTeam
{
    public class MembershipView
    {
        public string HashedAccountId { get; set; }
        public long AccountId { get; set; }
        public string AccountName { get; set; }
        public long UserId { get; set; }
        public Guid ExternalUserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Role Role { get; set; }

        public int RoleId
        {
            get => (int)Role;
            set => Role = (Role) Enum.Parse(typeof(Role), value.ToString());
        }

        public DateTime CreatedDate { get; set; }
        public bool ShowWizard { get; set; }
        public string FullName() => $"{FirstName} {LastName}";
    }
}