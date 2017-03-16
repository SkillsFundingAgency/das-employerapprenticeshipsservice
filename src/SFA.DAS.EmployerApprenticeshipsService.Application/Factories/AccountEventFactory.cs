using SFA.DAS.EAS.Account.Api.Types.Events;

namespace SFA.DAS.EAS.Application.Factories
{
    public class AccountEventFactory : IAccountEventFactory
    {
        public AccountCreatedEvent CreateAccountCreatedEvent(string hashedAccountId)
        {
            return new AccountCreatedEvent
            {
                Event = "AccountCreated",
                ResourceUri = $"api/accounts/{hashedAccountId}"
            };
        }

        public AccountRenamedEvent CreateAccountRenamedEvent(string hashedAccountId)
        {
            return new AccountRenamedEvent
            {
                Event = "AccountRenamed",
                ResourceUri = $"api/accounts/{hashedAccountId}"
            };
        }
    }
}
