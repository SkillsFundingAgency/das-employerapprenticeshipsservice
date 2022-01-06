using SFA.DAS.EmployerFinance.Interfaces;

namespace SFA.DAS.EmployerFinance.Configuration
{
    public class CommitmentsApiV2ClientConfiguration : IManagedIdentityClientConfiguration
    {
        public string ApiBaseUrl { get; set; }      
        public string IdentifierUri { get; set; }
    }
   
}
