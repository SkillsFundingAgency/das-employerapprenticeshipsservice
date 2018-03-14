using MediatR;
using Moq;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Application.Queries.GetAccountPayeSchemes;
using SFA.DAS.EAS.Application.Queries.GetUserAccounts;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.TestCommon.DependencyResolution;
using SFA.DAS.EAS.TestCommon.ScenarioCommonSteps;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Messaging.Interfaces;
using StructureMap;
using System;
using System.Linq;
using System.Web;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.Levy.HmrcScenarios.AcceptanceTests2.Steps.CommonSteps
{
    [Binding]
    public class AccountCreationSteps
    {
        private IContainer _container;
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

            _container = IoC.CreateContainer(_messagePublisher, _owinWrapper, _cookieService, _eventsApi, _commitmentsApi);

            _container.Inject(_validator.Object);

            _testData = new PaymentTestData(_container);
        }

        [Given(@"I have an account")]
        public void GivenIHaveAnAccount()
        {
            CreateAccountWithOwner();

            SetAccountIdForUser();

            //Create a new user with passed in role

            _externalUserId = Guid.NewGuid().ToString();
            ScenarioContext.Current["ExternalUserId"] = _externalUserId;

            CreateUserWithRole("Owner");
        }

        [Given(@"I add a PAYE Scheme (.*) with name (.*) to the account")]
        public void GivenIAddAPayeSchemeToTheAccount(string scheme, string schemeName)
        {
            var orchestrator = _container.GetInstance<EmployerAccountPayeOrchestrator>();

            var hashedAccountId = (string)ScenarioContext.Current["HashedAccountId"];

            var accountOwnerId = ScenarioContext.Current["AccountOwnerUserRef"].ToString();

            var viewModel = new AddNewPayeSchemeViewModel
            {
                PayeScheme = scheme,
                PayeName = schemeName,
                HashedAccountId = hashedAccountId,
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString()
            };

            orchestrator.AddPayeSchemeToAccount(viewModel, accountOwnerId).Wait();
        }


        public static void CreateDasAccount(UserViewModel userView, EmployerAccountOrchestrator orchestrator)
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

        private void CreateUserWithRole(string accountRole)
        {
            var accountId = (long)ScenarioContext.Current["AccountId"];
            Role roleOut;
            Enum.TryParse(accountRole, out roleOut);

            var signInModel = new UserViewModel
            {
                Email = "test@test.com" + Guid.NewGuid().ToString().Substring(0, 6),
                FirstName = "test",
                LastName = "tester",
                UserRef = _externalUserId
            };
            var userCreation = new UserSteps();
            userCreation.UpsertUser(signInModel);

            userCreation.CreateUserWithRole(
                new User
                {
                    Email = signInModel.Email,
                    FirstName = signInModel.FirstName,
                    LastName = signInModel.LastName,
                    UserRef = _externalUserId
                }, roleOut, accountId);
        }

        private void CreateAccountWithOwner()
        {
            var accountOwnerUserId = Guid.NewGuid().ToString();

            ScenarioContext.Current["AccountOwnerUserRef"] = accountOwnerUserId;

            var signInUserModel = new UserViewModel
            {
                UserRef = accountOwnerUserId,
                Email = "accountowner@test.com" + Guid.NewGuid().ToString().Substring(0, 6),
                FirstName = "Test",
                LastName = "Tester"
            };

            var userCreationSteps = new UserSteps();

            userCreationSteps.UpsertUser(signInUserModel);

            var user = userCreationSteps.GetExistingUserAccount();

            CreateDasAccount(user, _container.GetInstance<EmployerAccountOrchestrator>());

            ScenarioContext.Current["AccountOwnerUserId"] = user.Id;
        }

        private void SetAccountIdForUser()
        {
            var accountOwnerId = ScenarioContext.Current["AccountOwnerUserRef"].ToString();
            var mediator = _container.GetInstance<IMediator>();
            var getUserAccountsQueryResponse = mediator.SendAsync(new GetUserAccountsQuery { UserRef = accountOwnerId }).Result;

            var account = getUserAccountsQueryResponse.Accounts.AccountList.FirstOrDefault();

            _testData.AccountId = account?.Id ?? default(long);

            ScenarioContext.Current["HashedAccountId"] = account.HashedId;
        }
    }
}
