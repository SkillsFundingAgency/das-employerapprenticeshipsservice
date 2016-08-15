using System;
using System.Linq;
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

namespace SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.Steps.AddPayeScheme
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
        private string _accountOwnerUserId;
        private bool _newLegalEntity;
        private int _exceptionCount;

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
            _exceptionCount = 0;
            _newLegalEntity = false;

            _cleanDownDb.Execute().Wait();

            _container.Dispose();
        }

        [Given(@"I am an account ""(.*)""")]
        public void GivenIAmAnAccount(string accountRole)
        {
            
            CreateAccountWithOwner();

            SetAccountIdForUser();
            
            //Create a new user with passed in role
            CreateUserWithRole(accountRole);
        }
        
        [Then(@"I can view all of my PAYE schemes")]
        public void ThenICanViewAllOfMyPAYESchemes()
        {
            var employerPayeOrchestrator = _container.GetInstance<EmployerAccountPayeOrchestrator>();
            var legalEntities = employerPayeOrchestrator.Get(_accountId, _externalUserId).Result;

            Assert.AreEqual(1, legalEntities.Data.PayeSchemes.Count);
        }

        [When(@"I Add a new PAYE scheme to my existing legal entity")]
        public void WhenIAddANewPAYESchemeToMyExistingLegalEntity()
        {
            _newLegalEntity = false;

            var employerPayeOrchestrator = _container.GetInstance<EmployerAccountPayeOrchestrator>();

            var legalEntity = employerPayeOrchestrator.Get(_accountId, _externalUserId).Result.Data.PayeSchemes.FirstOrDefault();

            var confirmNewPayeScheme = new ConfirmNewPayeScheme
            {
                AccountId = _accountId,
                LegalEntityId = legalEntity.LegalEntityId,
                PayeScheme = "654/ABC",
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString()
            };
            try
            {
                employerPayeOrchestrator.AddPayeSchemeToAccount(confirmNewPayeScheme, _externalUserId).Wait();
            }
            catch (Exception)
            {
                _exceptionCount++;
            }
        }

        [When(@"I Add a new PAYE scheme to my new legal entity")]
        public void WhenIAddANewPAYESchemeToMyNewLegalEntity()
        {
            _newLegalEntity = true;
            var employerPayeOrchestrator = _container.GetInstance<EmployerAccountPayeOrchestrator>();

            var confirmNewPayeScheme = new ConfirmNewPayeScheme
            {
                AccountId = _accountId,
                LegalEntityId = 0,
                PayeScheme = "654/ABC",
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
                LegalEntityCode = "12345",
                LegalEntityDateOfIncorporation = new DateTime(2016,10,30),
                LegalEntityName = "Test Entity",
                LegalEntityRegisteredAddress = "Test Address"
            };
            try
            {
                employerPayeOrchestrator.AddPayeSchemeToAccount(confirmNewPayeScheme, _externalUserId).Wait();
            }
            catch (Exception)
            {
                _exceptionCount ++;
            }
            
        }


        [Then(@"The PAYE scheme Is ""(.*)""")]
        public void ThenThePAYESchemeIs(string schemeStatus)
        {
            //Get the PAYE schemes
            var employerPayeOrchestrator = _container.GetInstance<EmployerAccountPayeOrchestrator>();
            var legalEntities = employerPayeOrchestrator.Get(_accountId, _externalUserId).Result;
            var entities = legalEntities.Data.PayeSchemes.Select(c => c.LegalEntityId).Distinct().Count();
            if (schemeStatus.Equals("created", StringComparison.CurrentCultureIgnoreCase))
            {
                Assert.AreEqual(_newLegalEntity ? 2 : 1, entities);
                Assert.AreEqual(2, legalEntities.Data.PayeSchemes.Count);
                Assert.AreEqual(0, _exceptionCount);
            }
            else
            {
                Assert.AreEqual(1, entities);
                Assert.AreEqual(1, legalEntities.Data.PayeSchemes.Count);
                Assert.AreEqual(1, _exceptionCount);
            }

        }


        private void CreateUserWithRole(string accountRole)
        {
            var mediator = _container.GetInstance<IMediator>();
            var userRepository = _container.GetInstance<IUserRepository>();
            var membershipRepository = _container.GetInstance<IMembershipRepository>();

            Role roleOut;
            Enum.TryParse(accountRole, out roleOut);
            _externalUserId = Guid.NewGuid().ToString();

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
                }, roleOut, _accountId, userRepository, membershipRepository);
        }

        private void CreateAccountWithOwner()
        {
            _accountOwnerUserId = Guid.NewGuid().ToString();
            var signInUserModel = new SignInUserModel
            {
                UserId = _accountOwnerUserId,
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
            var getUserAccountsQueryResponse = mediator.SendAsync(new GetUserAccountsQuery { UserId = _accountOwnerUserId }).Result;

            _accountId = getUserAccountsQueryResponse.Accounts.AccountList.FirstOrDefault().Id;
        }
    }
}
