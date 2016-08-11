namespace SFA.DAS.EmployerApprenticeshipsService.Domain
{
    public class Paye
    {
        public string EmpRef { get; set; }
        public long AccountId { get; set; }  

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}