namespace SFA.DAS.EAS.Domain.Models.PAYE
{
    public class Paye
    {
        public string EmpRef { get; set; }
        public long AccountId { get; set; }  

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string RefName { get; set; }
    }
}