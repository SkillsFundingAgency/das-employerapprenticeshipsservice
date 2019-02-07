using System;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Models.UserProfile;

namespace SFA.DAS.EmployerAccounts.Models.AccountTeam
{
    public class Membership
    {
        public virtual Account.Account Account { get; set; }
        public virtual long AccountId { get; set; }
        public virtual User User { get; set; }
        public virtual long UserId { get; set; }

        public virtual Role Role
        {
            get => _role ?? (Role?)_roleId.Value ?? Role.None;
            set => _role = value;
        }

        // BUG: this is marked obsolete but the Role property relies on the RoleId property being set.
        [Obsolete("Please use 'Role' instead.")]
        public int RoleId
        {
            get => _roleId ?? (int?)_role.Value ?? (int)Role.None;
            set => _roleId = value;
        }

        private Role? _role;
        private int? _roleId;
    }
}