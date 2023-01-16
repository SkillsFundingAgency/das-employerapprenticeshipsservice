namespace SFA.DAS.EmployerAccounts.Commands.RemoveLegalEntity;

public class RemoveLegalEntityCommand : IRequest
{
    public string HashedAccountId { get; set; }
    public string HashedAccountLegalEntityId { get; set; }
    public string UserId { get; set; }
}