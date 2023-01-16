namespace SFA.DAS.EmployerAccounts.Commands.UnsubscribeProviderEmail;

public class UnsubscribeProviderEmailQuery : IRequest
{
    public Guid CorrelationId { get; set; }
}