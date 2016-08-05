namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account
{
    public class Account
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public int RoleId { get; set; }
        public string RoleName => ((RoleName)RoleId).ToString();
    }
}
