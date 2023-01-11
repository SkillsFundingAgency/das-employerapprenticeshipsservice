namespace SFA.DAS.EmployerAccounts.Commands.RenameEmployerAccount;

public class RenameEmployerAccountCommand : IAsyncRequest
{
    public string HashedAccountId { get; set; }
    public string ExternalUserId { get; set; }
    public string NewName { get; set; }
}