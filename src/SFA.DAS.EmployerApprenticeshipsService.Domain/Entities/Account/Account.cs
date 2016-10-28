namespace SFA.DAS.EAS.Domain.Entities.Account
{
    public class Account
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public string HashedId { get; set; }
        public int RoleId { get; set; }
        public string RoleName => ((Role)RoleId).ToString();
    }
}
