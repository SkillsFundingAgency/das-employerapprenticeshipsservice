using SFA.DAS.EAS.Account.Api.Types.Events.Account;

namespace SFA.DAS.EAS.Application.Factories
{
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
}
