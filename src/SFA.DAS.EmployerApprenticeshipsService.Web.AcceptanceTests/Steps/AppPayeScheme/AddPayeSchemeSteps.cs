using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUserAccounts;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.DbCleanup;
using SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.DependencyResolution;
using SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.Steps.CommonSteps;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.Messaging;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.Steps.AppPayeScheme
{
    [Binding, Explicit]
    public class AddPayeSchemeSteps
    {
        private IContainer _container;
        private string _externalUserId;
        private long _accountId;
        private Mock<IMessagePublisher> _messagePublisher;
        private ICleanDatabase _cleanDownDb;
        private Mock<IOwinWrapper> _owinWrapper;


        [BeforeScenario]
        public void Arrange()
        {
            _messagePublisher = new Mock<IMessagePublisher>();
            _owinWrapper = new Mock<IOwinWrapper>();

            _container = IoC.CreateContainer(_messagePublisher, _owinWrapper);

            _cleanDownDb = _container.GetInstance<ICleanDatabase>();
            _cleanDownDb.Execute().Wait();
        }

        [AfterScenario]
        public void TearDown()
        {
            _cleanDownDb.Execute().Wait();

            _container.Dispose();
        }

        [Given(@"I am an account ""(.*)""")]
        public void GivenIAmAnAccount(string accountRole)
        {
            ScenarioContext.Current.Pending();
            //CreateAccountWithOwner();

            //SetAccountIdForUser();

            //var mediator = _container.GetInstance<IMediator>();
            //var userRepository = _container.GetInstance<IUserRepository>();
            //var membershipRepository = _container.GetInstance<IMembershipRepository>();

            //var user = UserCreationSteps.GetExistingUserAccount(_container.GetInstance<HomeOrchestrator>());

            //UserCreationSteps.UpsertUser(user, mediator);

            //AccountCreationSteps.CreateDasAccount(user, _container.GetInstance<EmployerAccountOrchestrator>());

            ////Create a new user with passed in role
            //_externalUserId = Guid.NewGuid().ToString();

            //var signInModel = new SignInUserModel
            //{
            //    Email = "test@test.com",
            //    FirstName = "test",
            //    LastName = "tester",
            //    UserId = _externalUserId
            //};

            //UserCreationSteps.UpsertUser(signInModel, mediator);
            //Role roleOut;
            //Enum.TryParse(accountRole, out roleOut);
            //UserCreationSteps.CreateUserWithRole(new User {Email = signInModel.Email, FirstName = signInModel.FirstName, LastName = signInModel.LastName, UserRef = _externalUserId}, roleOut,_accountId, userRepository,membershipRepository);

        }

        [Given(@"I am part of an account")]
        public void GivenIAmPartOfAnAccount()
        {
            ScenarioContext.Current.Pending();
            //var user = UserCreationSteps.GetExistingUserAccount(_container.GetInstance<HomeOrchestrator>());

            //UserCreationSteps.UpsertUser(user, _container.GetInstance<IMediator>());

            //AccountCreationSteps.CreateDasAccount(user, _container.GetInstance<EmployerAccountOrchestrator>());
        }

        [When(@"I view PAYE schemes")]
        public void WhenIViewPAYESchemes()
        {
            ScenarioContext.Current.Pending();
        }



        [When(@"I Add a new PAYE scheme to my existing legal entity")]
        public void WhenIAddANewPAYESchemeToMyExistingLegalEntity()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"I Add a new PAYE scheme to my new legal entity")]
        public void WhenIAddANewPAYESchemeToMyNewLegalEntity()
        {
            ScenarioContext.Current.Pending();
        }


        [Then(@"The PAYE scheme Is ""(.*)""")]
        public void ThenThePAYESchemeIs(string schemeStatus)
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"All schemes are returned")]
        public void ThenAllSchemesAreReturned()
        {
            ScenarioContext.Current.Pending();
        }

        private void CreateAccountWithOwner()
        {
            var signInUserModel = new SignInUserModel
            {
                UserId = Guid.NewGuid().ToString(),
                Email = "accountowner@test.com",
                FirstName = "Test",
                LastName = "Tester"
            };
            UserCreationSteps.UpsertUser(signInUserModel, _container.GetInstance<IMediator>());

            var user = UserCreationSteps.GetExistingUserAccount(_container.GetInstance<HomeOrchestrator>());
            
            AccountCreationSteps.CreateDasAccount(user, _container.GetInstance<EmployerAccountOrchestrator>());
        }

        private void SetAccountIdForUser()
        {
            var mediator = _container.GetInstance<IMediator>();
            var getUserAccountsQueryResponse = mediator.SendAsync(new GetUserAccountsQuery { UserId = _externalUserId }).Result;

            _accountId = getUserAccountsQueryResponse.Accounts.AccountList.FirstOrDefault().Id;
        }
    }
}
