namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface IProviderRegistrationApiClient
{
    Task Unsubscribe(string CorrelationId);

    Task<string> GetInvitations(string CorrelationId);
}