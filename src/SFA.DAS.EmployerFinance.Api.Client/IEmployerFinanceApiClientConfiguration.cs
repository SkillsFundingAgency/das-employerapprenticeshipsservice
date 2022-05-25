namespace SFA.DAS.EmployerFinance.Api.Client
{
    public interface IEmployerFinanceApiClientConfiguration
    {
        string ApiBaseUrl { get; }
        string IdentifierUri { get; }
    }
}