using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper.Dtos;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.ModelBuilders
{

    public class TestModelBuilder
    {
        public static UserModelBuilder User = new UserModelBuilder();
        public static AccountModelBuilder Account = new AccountModelBuilder();
        public static LegalEntityModelBuilder LegalEntity = new LegalEntityModelBuilder();

        private readonly List<UserSetup> _users = new List<UserSetup>();

        public TestModelBuilder WithNewUser()
        {
            var userSetup = new UserSetup
            {
                UserInput = User.CreateUserInput()
            };

            _users.Add(userSetup);

            return this;
        }

        public ICollection<UserSetup> Users => _users;

        public UserSetup CurrentUser => _users.Last();
        public EmployerAccountSetup CurrentAccount => CurrentUser.Accounts.Last();
        public LegalEntityWithAgreementSetup CurrentLegalEntity => CurrentAccount.LegalEntities.Last();


        public TestModelBuilder WithNewAccount(string accountName, string payeRef)
        {
            Contract.Assert(HasCurrentUser, "Add a user before adding an account");

            var currentUser = CurrentUser;

            var account = new EmployerAccountSetup
            {
                AccountInput = Account.CreateAccountInput(accountName, payeRef, () => currentUser.UserOutput.UserId)
            };

            currentUser.Accounts.Add(account);

            return this;
        }

        public TestModelBuilder WithNewLegalEntity(string legalEntityName)
        {
            Contract.Assert(HasCurrentAccount, "Add an account before adding a legal entity");

            var currentAccount = CurrentAccount;

            var legalEntity = new LegalEntityWithAgreementSetup
            {
                LegalEntityWithAgreementInputInput = LegalEntity.BuildEntityWithAgreementInput(legalEntityName, () => currentAccount.AccountOutput.AccountId)
            };

            currentAccount.LegalEntities.Add(legalEntity);

            return this;
        }

        public bool HasCurrentUser => _users.Count > 0;
        public bool HasCurrentAccount => HasCurrentUser && _users.Last().Accounts.Count > 0;
        public bool HasCurrentLegalEntity => HasCurrentAccount && _users.Last().Accounts.Last().LegalEntities.Count > 0;
    }
}
