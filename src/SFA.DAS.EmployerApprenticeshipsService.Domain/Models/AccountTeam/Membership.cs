using System;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Domain.Models.AccountTeam
{
    public class Membership
    {
        public virtual Account.Account Account { get; set; }
        public virtual long AccountId { get; set; }
        public virtual User User { get; set; }
        public virtual long UserId { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual Role Role { get; set; }
    }
}