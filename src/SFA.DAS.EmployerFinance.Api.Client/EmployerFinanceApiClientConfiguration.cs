namespace SFA.DAS.EmployerFinance.Api.Client
{
    public class EmployerFinanceApiClientConfiguration : IEmployerFinanceApiClientConfiguration
    {
        public string ApiBaseUrl { get; }
        public string IdentifierUri { get; set; }
    }
}