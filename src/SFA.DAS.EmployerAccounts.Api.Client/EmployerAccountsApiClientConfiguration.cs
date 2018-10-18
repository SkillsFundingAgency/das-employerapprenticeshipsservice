namespace SFA.DAS.EmployerAccounts.Api.Client
{
    public class EmployerAccountsApiClientConfiguration : IEmployerAccountsApiClientConfiguration
    {
        public string ApiBaseUrl { get; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
        public string Tenant { get; set; }
    }
}