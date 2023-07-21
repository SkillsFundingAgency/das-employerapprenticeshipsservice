namespace SFA.DAS.EmployerAccounts.Commands.UpsertRegisteredUser;

public class UpsertRegisteredUserCommand : IRequest
{
    public string UserRef { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string CorrelationId { get; set; }
}