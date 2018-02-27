using System;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Domain.Models.AccountTeam
{
    public class Membership
    {
        private int? _roleId;
        private Role? _role;

        public long AccountId { get; set; }
        public long UserId { get; set; }

        [Obsolete("Please use 'Role' instead.")]
        public int RoleId
        {
            get => _roleId ?? (int)_role.Value;
            set => _roleId = value;
        }

        public Role Role
        {
            get => _role ?? (Role)_roleId.Value;
            set => _role = value;
        }

        public Data.Entities.Account.Account Account { get; set; }
        public User User { get; set; }
    }
}