using SFA.DAS.EmployerAccounts.Events.Account;

namespace SFA.DAS.EmployerAccounts.Factories;

public class AccountEventFactory : IAccountEventFactory
{
    public AccountCreatedEvent CreateAccountCreatedEvent(string hashedAccountId)
    {
        return new AccountCreatedEvent
        {
            ResourceUri = $"api/accounts/{hashedAccountId}"
        };
    }

    public AccountRenamedEvent CreateAccountRenamedEvent(string hashedAccountId)
    {
        return new AccountRenamedEvent
        {
            ResourceUri = $"api/accounts/{hashedAccountId}"
        };
    }
}