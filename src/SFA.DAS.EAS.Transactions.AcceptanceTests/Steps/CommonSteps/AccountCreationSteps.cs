using MediatR;
using Moq;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Application.Queries.GetAccountPayeSchemes;
using SFA.DAS.EAS.Application.Queries.GetUserAccounts;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.TestCommon.DependencyResolution;
using SFA.DAS.EAS.TestCommon.ScenarioCommonSteps;
using SFA.DAS.EAS.TestCommon.TestModels;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Messaging.Interfaces;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.Transactions.AcceptanceTests.Steps.CommonSteps
{
    [Binding]
    public class AccountCreationSteps
    {
        public IContainer Container { get; }

        private Mock<IMessagePublisher> _messagePublisher;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;

        private string _externalUserId;
        private Mock<IValidator<GetAccountPayeSchemesQuery>> _validator;
        private Mock<IEventsApi> _eventsApi;
        private PaymentTestData _testData;
        private Mock<IEmployerCommitmentApi> _commitmentsApi;

        public AccountCreationSteps()
        {
            _messagePublisher = new Mock<IMessagePublisher>();
            _owinWrapper = new Mock<IAuthenticationService>();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            _eventsApi = new Mock<IEventsApi>();
            _validator = new Mock<IValidator<GetAccountPayeSchemesQuery>>();
            _commitmentsApi = new Mock<IEmployerCommitmentApi>();

            _validator.Setup(x => x.ValidateAsync(It.IsAny<GetAccountPayeSchemesQuery>()))
                .ReturnsAsync(new ValidationResult());

            Container = IoC.CreateContainer(_messagePublisher, _owinWrapper, _cookieService, _eventsApi, _commitmentsApi);

            Container.Inject(_validator.Object);

            _testData = new PaymentTestData(Container);
        }

        [Given(@"I have an account")]
        public void GivenIHaveAnAccount()
        {
            var user = CreateUser();

            ScenarioContext.Current["AccountOwnerUserRef"] = user.UserRef;
            ScenarioContext.Current["AccountOwnerUserId"] = user.Id;
            ScenarioContext.Current["ExternalUserId"] = user.UserRef;

            var account = CreateAccount(user);

            _testData.AccountId = account?.Id ?? default(long);
            ScenarioContext.Current["AccountId"] = account?.Id;
            ScenarioContext.Current["HashedAccountId"] = account?.HashedId;
        }

        public TestUser CreateAccount()
        {
            var user = CreateUser();
            var account = CreateAccount(user);

            return new TestUser
            {
                Id = user.Id,
                UserRef = user.UserRef,
                Accounts = new List<TestAccount>
                {
                    new TestAccount
                    {
                        Id = account.Id,
                        HashedId = account.HashedId,
                        Name = account.Name
                    }
                }
            };
        }

        private UserViewModel CreateUser()
        {
            var userRef = Guid.NewGuid().ToString();

            var signInUserModel = new UserViewModel
            {
                UserRef = userRef,
                Email = "accountowner@test.com" + Guid.NewGuid().ToString().Substring(0, 6),
                FirstName = "Test",
                LastName = "Tester"
            };

            var userCreationSteps = new UserSteps();

            userCreationSteps.UpsertUser(signInUserModel);

            return userCreationSteps.GetExistingUserAccount(userRef);
        }

        private Account CreateAccount(UserViewModel owner)
        {
            CreateDasAccount(owner, Container.GetInstance<EmployerAccountOrchestrator>());

            var account = GetUserAccounts(owner).FirstOrDefault();

            return account;
        }

        private IEnumerable<Account> GetUserAccounts(UserViewModel user)
        {
            var mediator = Container.GetInstance<IMediator>();
            var getUserAccountsQueryResponse = mediator.SendAsync(new GetUserAccountsQuery { UserRef = user.UserRef }).Result;

            return getUserAccountsQueryResponse.Accounts.AccountList;
        }

        private static void CreateDasAccount(UserViewModel userView, EmployerAccountOrchestrator orchestrator)
        {
            orchestrator.CreateAccount(new CreateAccountViewModel
            {
                UserId = userView.UserRef,
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
                OrganisationDateOfInception = new DateTime(2016, 01, 01),
                PayeReference = $"{Guid.NewGuid().ToString().Substring(0, 3)}/{Guid.NewGuid().ToString().Substring(0, 7)}",
                OrganisationName = "Test Company",
                OrganisationReferenceNumber = "123456TGB" + Guid.NewGuid().ToString().Substring(0, 6),
                OrganisationAddress = "Address Line 1",
                OrganisationStatus = "active"
            }, new Mock<HttpContextBase>().Object).Wait();
        }
    }
}
