namespace SFA.DAS.EmployerFinance.Api.Client
{
    public interface IEmployerFinanceApiClientConfiguration
    {
        string ApiBaseUrl { get; }
        string ClientId { get; }
        string ClientSecret { get; }
        string IdentifierUri { get; }
        string Tenant { get; }
    }
}