namespace SFA.DAS.EmployerFinance.Api.Client
{
    public class EmployerFinanceApiClientConfiguration : IEmployerFinanceApiClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
        public string Tenant { get; set; }
    }
}