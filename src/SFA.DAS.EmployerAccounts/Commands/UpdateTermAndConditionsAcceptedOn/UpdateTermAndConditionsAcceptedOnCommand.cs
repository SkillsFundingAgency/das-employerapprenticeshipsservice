namespace SFA.DAS.EmployerAccounts.Commands.UpsertRegisteredUser;

public class UpdateTermAndConditionsAcceptedOnCommand : IAsyncRequest
{
    public string UserRef { get; set; }
}