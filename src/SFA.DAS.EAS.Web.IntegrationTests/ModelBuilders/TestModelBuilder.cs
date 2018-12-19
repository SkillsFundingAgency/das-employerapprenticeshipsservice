using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using AutoFixture;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.Dtos;
using SFA.DAS.EAS.Domain.Models.Payments;


namespace SFA.DAS.EAS.Account.API.IntegrationTests.ModelBuilders
{
    /// <summary>
    ///     This creates the information that can be used by <see cref="EmployerAccountsDbBuilder.SetupDataAsync"/>
    ///     to seed the database.
    /// </summary>
    public class TestModelBuilder
    {
        private readonly List<UserSetup> _users = new List<UserSetup>();

        private readonly Fixture _fixture;

        public ICollection<UserSetup> Users => _users;

        public UserSetup CurrentUser => _users.Last();
        public bool HasCurrentUser => _users.Count > 0;

        public EmployerAccountSetup CurrentAccount => CurrentUser.Accounts.Last();
        public bool HasCurrentAccount => HasCurrentUser && _users.Last().Accounts.Count > 0;
        public List<PaymentSetup> Payments { get; } 

        public TestModelBuilder()
        {
            _fixture = new Fixture();
            Payments = new List<PaymentSetup>();
        }

        /// <summary>
        ///     Add the information to create a new user. The user will be available in the <see cref="CurrentUser"/> but will not 
        ///     be persisted to the database until <see cref="EmployerAccountsDbBuilder.SetupDataAsync"/> is called.
        /// </summary>
        public TestModelBuilder WithNewUser()
        {
            var userSetup = new UserSetup
            {
                UserInput = _fixture.Create<UserInput>()
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
        public TestModelBuilder WithNewAccount()
        {
            Contract.Assert(HasCurrentUser, "Add a user before adding an account");

            var currentUser = CurrentUser;

            var account = _fixture
                .Build<EmployerAccountInput>()
                .With(input => input.UserId, () => currentUser.UserOutput.UserId)
                .Create();

            var accountSetup = new EmployerAccountSetup
            {
                AccountInput = account
            };

            currentUser.Accounts.Add(accountSetup);

            return this;
        }

        /// <summary>
        ///     Add the information to create a new legal entity.
        ///     The legal entity will be created for the account specified in <see cref="CurrentAccount"/>. An exception
        ///     will be thrown if an account has not been created (via <see cref="WithNewAccount"/>. The legal entity will not 
        ///     be persisted to the database until <see cref="EmployerAccountsDbBuilder.SetupDataAsync"/> is called.
        /// </summary>
        public TestModelBuilder WithNewLegalEntity()
        {
            Contract.Assert(HasCurrentAccount, "Add an account before adding a legal entity");

            var currentAccount = CurrentAccount;

            var legalEntity = _fixture
                .Build<LegalEntityWithAgreementInput>()
                .With(input => input.AccountId, () => currentAccount.AccountOutput.AccountId)
                .Create();

            var legalEntitySetup = new LegalEntityWithAgreementSetup
            {
                LegalEntityWithAgreementInput = legalEntity
            };

            currentAccount.LegalEntities.Add(legalEntitySetup);

            return this;
        }

        internal TestModelBuilder WithNewPayment()
        {
            var paymentDetails = _fixture
                .Build<PaymentDetails>()
                .With(details => details.CollectionPeriodId, "R05")
                // could put sanitised collection period and delivery period values in for mth and year
                .With(details => details.PeriodEnd, "R12")
                .With(details => details.EmployerAccountVersion, $"ver-{DateTime.Now.Ticks.ToString().Substring(4, 10)}")
                .With(details => details.ApprenticeshipVersion, $"ver-{DateTime.Now.Ticks.ToString().Substring(4, 10)}")
                .Without(details => details.FrameworkCode)
                .Without(details => details.PathwayCode)
                .Without(details => details.PathwayName)
                .Create();

            var paymentSetup = new PaymentSetup
            {
                PaymentInput = paymentDetails
            };
            Payments.Add(paymentSetup);
            
            return this;
        }
    }
}
