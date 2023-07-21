namespace SFA.DAS.EmployerAccounts.Commands.UnsubscribeProviderEmail;

public class UnsubscribeProviderEmailCommand : IRequest
{
    public Guid CorrelationId { get; set; }
}