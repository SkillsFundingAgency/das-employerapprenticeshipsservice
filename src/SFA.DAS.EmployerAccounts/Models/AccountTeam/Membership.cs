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
        public virtual DateTime CreatedDate { get; set; }
        public virtual Role Role
        {
            get => _role ?? (Role)_roleId.Value;
            set => _role = value;
        }

        [Obsolete("Please use 'Role' instead.")]
        public int RoleId
        {
            get => _roleId ?? (int)_role.Value;
            set => _roleId = value;
        }

        private Role? _role;
        private int? _roleId;
    }
}