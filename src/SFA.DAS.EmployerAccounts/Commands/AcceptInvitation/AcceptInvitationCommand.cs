namespace SFA.DAS.EmployerAccounts.Commands.AcceptInvitation;

public class AcceptInvitationCommand : IAsyncRequest
{
    public long Id { get; set; }
    public string ExternalUserId { get; set; }  
}