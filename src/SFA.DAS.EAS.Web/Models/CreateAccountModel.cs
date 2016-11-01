using System;

namespace SFA.DAS.EAS.Web.Models
{
    public class CreateAccountModel
    {
        public string UserId { get; set; }
        public string CompanyNumber { get; set; }
        public string CompanyName { get; set; }
        public string CompanyRegisteredAddress { get; set; }
        public DateTime CompanyDateOfIncorporation { get; set; }
        public string EmployerRef { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}