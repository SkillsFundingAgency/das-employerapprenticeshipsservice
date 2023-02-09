namespace SFA.DAS.EmployerAccounts.Commands.RemoveLegalEntity;

public class RemoveLegalEntityCommand : IRequest
{
    public long AccountId { get; set; }
    public long AccountLegalEntityId { get; set; }
    public string UserId { get; set; }
}