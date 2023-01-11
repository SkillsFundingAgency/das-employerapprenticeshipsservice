namespace SFA.DAS.EmployerAccounts.Commands.RemoveLegalEntity;

public class RemoveLegalEntityCommand : IAsyncRequest
{
    public string HashedAccountId { get; set; }
    public string HashedAccountLegalEntityId { get; set; }
    public string UserId { get; set; }
}