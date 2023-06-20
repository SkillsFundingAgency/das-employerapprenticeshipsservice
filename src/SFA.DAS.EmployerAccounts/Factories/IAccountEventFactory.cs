using SFA.DAS.EmployerAccounts.Events.Account;

namespace SFA.DAS.EmployerAccounts.Factories;

public interface IAccountEventFactory
{
    AccountCreatedEvent CreateAccountCreatedEvent(string hashedAccountId);
    AccountRenamedEvent CreateAccountRenamedEvent(string hashedAccountId);
}