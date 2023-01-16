namespace SFA.DAS.EmployerAccounts.Commands.RemoveTeamMember;

public class RemoveTeamMemberCommand : IRequest
{
    public long UserId { get; set; }
    public Guid UserRef { get; set; }
    public string HashedAccountId { get; set; }
    public string ExternalUserId { get; set; }
}