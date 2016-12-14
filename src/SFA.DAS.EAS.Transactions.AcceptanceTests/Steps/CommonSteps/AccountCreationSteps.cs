using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using SFA.DAS.EAS.Application.Queries.GetAccountPayeSchemes;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.TestCommon.DependencyResolution;
using SFA.DAS.EAS.TestCommon.ScenarioCommonSteps;
using SFA.DAS.EAS.Web;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.Messaging;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.Transactions.AcceptanceTests.Steps.CommonSteps
{
    [Binding]
    public class AccountCreationSteps
    {
        private IContainer _container;
        private Mock<IMessagePublisher> _messagePublisher;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<ICookieService> _cookieService;

        private string _externalUserId;
        private Mock<IValidator<GetAccountPayeSchemesQuery>> _validator;

        public AccountCreationSteps()
        {
            _messagePublisher = new Mock<IMessagePublisher>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _cookieService = new Mock<ICookieService>();
            _validator = new Mock<IValidator<GetAccountPayeSchemesQuery>>();

            _validator.Setup(x => x.ValidateAsync(It.IsAny<GetAccountPayeSchemesQuery>()))
                .ReturnsAsync(new ValidationResult());

            _container = IoC.CreateContainer(_messagePublisher, _owinWrapper, _cookieService);

            _container.Inject(_validator.Object);
        }

        [Given(@"I have an account")]
        public void GivenIHaveAnAccount()
        {
            var accountOwnerId = AccountSteps.CreateAccountWithOwner(_container.GetInstance<EmployerAccountOrchestrator>(), _container.GetInstance<IMediator>(), _owinWrapper, _container.GetInstance<HomeOrchestrator>());
            ScenarioContext.Current["AccountOwnerUserId"] = accountOwnerId.ToString();

            AccountSteps.SetAccountIdForUser(_container.GetInstance<IMediator>(), ScenarioContext.Current);//SetAccountIdForUser();

            //Create a new user with passed in role

            _externalUserId = Guid.NewGuid().ToString();
            ScenarioContext.Current["ExternalUserId"] = _externalUserId;

            CreateUserWithRole("Owner");
        }
        private void CreateUserWithRole(string accountRole)
        {
            var accountId = (long)ScenarioContext.Current["AccountId"];
            Role roleOut;
            Enum.TryParse(accountRole, out roleOut);

            var signInModel = new SignInUserModel
            {
                Email = "test@test.com" + Guid.NewGuid().ToString().Substring(0, 6),
                FirstName = "test",
                LastName = "tester",
                UserId = _externalUserId
            };
            //var userCreation = new UserCreationSteps();
            UserSteps.UpsertUser(_container.GetInstance<IMediator>(), signInModel);

            UserSteps.CreateUserWithRole(
                new User
                {
                    Email = signInModel.Email,
                    FirstName = signInModel.FirstName,
                    LastName = signInModel.LastName,
                    UserRef = _externalUserId
                }, roleOut, accountId, _container.GetInstance<IUserRepository>(), _container.GetInstance<IMembershipRepository>());
        }
    }
}
