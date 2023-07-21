namespace SFA.DAS.EmployerAccounts.Commands.DeleteInvitation;

public class DeleteInvitationCommand : IRequest
{
    public string Email { get; set; }
    public string HashedAccountId { get; set; }
    public string ExternalUserId { get; set; }
}