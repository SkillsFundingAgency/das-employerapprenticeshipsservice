namespace SFA.DAS.EmployerAccounts.Commands.UpsertRegisteredUser;

public class UpdateTermAndConditionsAcceptedOnCommand : IRequest
{
    public string UserRef { get; set; }
}