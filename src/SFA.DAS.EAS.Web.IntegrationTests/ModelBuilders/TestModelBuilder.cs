using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper.Dtos;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.ModelBuilders
{
    /// <summary>
    ///     This creates the information that can be used by <see cref="EmployerAccountsDbBuilder.SetupDataAsync"/>
    ///     to seed the database.
    /// </summary>
    public class TestModelBuilder
    {
        private readonly UserModelBuilder _user = new UserModelBuilder();
        private readonly AccountModelBuilder _account = new AccountModelBuilder();
        private readonly LegalEntityModelBuilder _legalEntity = new LegalEntityModelBuilder();
        private readonly List<UserSetup> _users = new List<UserSetup>();

        public ICollection<UserSetup> Users => _users;

        public UserSetup CurrentUser => _users.Last();
        public bool HasCurrentUser => _users.Count > 0;

        public EmployerAccountSetup CurrentAccount => CurrentUser.Accounts.Last();
        public bool HasCurrentAccount => HasCurrentUser && _users.Last().Accounts.Count > 0;

        /// <summary>
        ///     Add the information to create a new user. The user will be available in the <see cref="CurrentUser"/> but will not 
        ///     be persisted to the database until <see cref="EmployerAccountsDbBuilder.SetupDataAsync"/> is called.
        /// </summary>
        public TestModelBuilder WithNewUser()
        {
            var userSetup = new UserSetup
            {
                UserInput = _user.CreateUserInput()
            };

            _users.Add(userSetup);

            return this;
        }

        /// <summary>
        ///     Add the information to create a new account. The account will be available in the <see cref="CurrentAccount"/>.
        ///     The account will be created for the user specified in <see cref="CurrentUser"/>. An exception
        ///     will be thrown if a user has not been created (via <see cref="WithNewUser"/>. The account will not 
        ///     be persisted to the database until <see cref="EmployerAccountsDbBuilder.SetupDataAsync"/> is called.
        /// </summary>
        public TestModelBuilder WithNewAccount(string accountName, string payeRef)
        {
            Contract.Assert(HasCurrentUser, "Add a user before adding an account");

            var currentUser = CurrentUser;

            var account = new EmployerAccountSetup
            {
                AccountInput = _account.CreateAccountInput(accountName, payeRef, () => currentUser.UserOutput.UserId)
            };

            currentUser.Accounts.Add(account);

            return this;
        }

        /// <summary>
        ///     Add the information to create a new legal entity.
        ///     The legal entity will be created for the account specified in <see cref="CurrentAccount"/>. An exception
        ///     will be thrown if an account has not been created (via <see cref="WithNewAccount"/>. The legal entity will not 
        ///     be persisted to the database until <see cref="EmployerAccountsDbBuilder.SetupDataAsync"/> is called.
        /// </summary>
        public TestModelBuilder WithNewLegalEntity(string legalEntityName)
        {
            Contract.Assert(HasCurrentAccount, "Add an account before adding a legal entity");

            var currentAccount = CurrentAccount;

            var legalEntity = new LegalEntityWithAgreementSetup
            {
                LegalEntityWithAgreementInputInput = _legalEntity.BuildEntityWithAgreementInput(legalEntityName, () => currentAccount.AccountOutput.AccountId)
            };

            currentAccount.LegalEntities.Add(legalEntity);

            return this;
        }
    }
}
