using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUserAccounts;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.DependencyResolution;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.Messaging;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.Steps.CommonSteps
{
    [Binding]
    public class AccountCreationSteps
    {
        private IContainer _container;
        private Mock<IMessagePublisher> _messagePublisher;
        private Mock<IOwinWrapper> _owinWrapper;
        public AccountCreationSteps()
        {
            _messagePublisher = new Mock<IMessagePublisher>();
            _owinWrapper = new Mock<IOwinWrapper>();

            _container = IoC.CreateContainer(_messagePublisher, _owinWrapper);
        }
        private string _externalUserId;

        [Given(@"I am an account ""(.*)""")]
        public void GivenIAmAnAccount(string accountRole)
        {

            CreateAccountWithOwner();

            SetAccountIdForUser();

            //Create a new user with passed in role

            _externalUserId = Guid.NewGuid().ToString();
            ScenarioContext.Current["ExternalUserId"] = _externalUserId;

            CreateUserWithRole(accountRole);
        }

        public static void CreateDasAccount(SignInUserModel user, EmployerAccountOrchestrator orchestrator)
        {

            orchestrator.CreateAccount(new CreateAccountModel
            {
                UserId = user.UserId,
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
                CompanyDateOfIncorporation = new DateTime(2016, 01, 01),
                EmployerRef = "123/ABC",
                CompanyName = "Test Company",
                CompanyNumber = "123456TGB",
                CompanyRegisteredAddress = "Address Line 1"
            }).Wait();


        }

        private void CreateUserWithRole(string accountRole)
        {
            var accountId = (long)ScenarioContext.Current["AccountId"];
            var mediator = _container.GetInstance<IMediator>();
            var userRepository = _container.GetInstance<IUserRepository>();
            var membershipRepository = _container.GetInstance<IMembershipRepository>();

            Role roleOut;
            Enum.TryParse(accountRole, out roleOut);

            var signInModel = new SignInUserModel
            {
                Email = "test@test.com",
                FirstName = "test",
                LastName = "tester",
                UserId = _externalUserId
            };

            UserCreationSteps.UpsertUser(signInModel, mediator);

            UserCreationSteps.CreateUserWithRole(
                new User
                {
                    Email = signInModel.Email,
                    FirstName = signInModel.FirstName,
                    LastName = signInModel.LastName,
                    UserRef = _externalUserId
                }, roleOut, accountId, userRepository, membershipRepository);
        }

        private void CreateAccountWithOwner()
        {
            var accountOwnerUserId = Guid.NewGuid().ToString();
            ScenarioContext.Current["AccountOwnerUserId"] = accountOwnerUserId;

            var signInUserModel = new SignInUserModel
            {
                UserId = accountOwnerUserId,
                Email = "accountowner@test.com",
                FirstName = "Test",
                LastName = "Tester"
            };
            UserCreationSteps.UpsertUser(signInUserModel, _container.GetInstance<IMediator>());

            var user = UserCreationSteps.GetExistingUserAccount(_container.GetInstance<HomeOrchestrator>());

            CreateDasAccount(user, _container.GetInstance<EmployerAccountOrchestrator>());
        }

        private void SetAccountIdForUser()
        {
            var accountOwnerId = ScenarioContext.Current["AccountOwnerUserId"].ToString();
            var mediator = _container.GetInstance<IMediator>();
            var getUserAccountsQueryResponse = mediator.SendAsync(new GetUserAccountsQuery { UserId = accountOwnerId }).Result;

            ScenarioContext.Current["AccountId"] = getUserAccountsQueryResponse.Accounts.AccountList.FirstOrDefault().Id;
        }
    }
}
