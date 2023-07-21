namespace SFA.DAS.EmployerAccounts.Commands.ResendInvitation;

public class ResendInvitationCommand : IRequest
{
    public string Email { get; set; }
    public string AccountId { get; set; }
    public string ExternalUserId { get; set; }
    public string FirstName { get; set; }
}