namespace SFA.DAS.EAS.Domain
{
    public class MembershipView
    {
        public string HashedId { get; set; }
        public long AccountId { get; set; }
        public string AccountName { get; set; }
        public long UserId { get; set; }
        public string UserRef { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}