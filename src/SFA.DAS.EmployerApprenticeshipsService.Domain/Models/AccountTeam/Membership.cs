using System;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Domain.Models.AccountTeam
{
    public class Membership
    {
        public virtual Account.Account Account { get; set; }
        public virtual long AccountId { get; set; }
        public virtual User User { get; set; }
        public virtual long UserId { get; set; }

        public virtual Role Role
        {
            get => _role ?? Role.None;
            set => _role = value;
        }

        //[Obsolete("Please use 'Role' instead.")]
        //public int RoleId
        //{
        //    set
        //    {
        //        Role = Role.None;
        //        if (Enum.TryParse(value.ToString(), out Role role))
        //        {
        //            Role = role;
        //        }
        //    }
        //}

        private Role? _role;
        //private int? _roleId;
    }
}