using SFA.DAS.EmployerAccounts.Events.Account;

namespace SFA.DAS.EAS.Application.Factories
{
    public interface IAccountEventFactory
    {
        AccountCreatedEvent CreateAccountCreatedEvent(string hashedAccountId);
        AccountRenamedEvent CreateAccountRenamedEvent(string hashedAccountId);
    }
}
