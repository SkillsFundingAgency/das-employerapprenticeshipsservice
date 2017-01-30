namespace SFA.DAS.EAS.Domain.Models.AccountTeam
{
    public class Membership
    { 
        public long AccountId { get; set; }
        public long UserId { get; set; }
        public int RoleId { get; set; }
    }
}