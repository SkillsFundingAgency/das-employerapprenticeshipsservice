namespace SFA.DAS.EmployerAccounts.Api.Client
{
    public interface IEmployerAccountsApiClientConfiguration
    {
        string ApiBaseUrl { get; }
        string ClientId { get; }
        string ClientSecret { get; }
        string IdentifierUri { get; }
        string Tenant { get; }
    }
}