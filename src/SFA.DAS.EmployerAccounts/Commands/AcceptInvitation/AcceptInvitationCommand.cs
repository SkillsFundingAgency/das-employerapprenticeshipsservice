namespace SFA.DAS.EmployerAccounts.Commands.AcceptInvitation;

public class AcceptInvitationCommand : IRequest
{
    public long Id { get; set; }
    public string ExternalUserId { get; set; }  
}