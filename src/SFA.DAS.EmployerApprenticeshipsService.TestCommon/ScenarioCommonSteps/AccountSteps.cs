using System;
using System.Linq;
using System.Web;
using MediatR;
using Moq;
using SFA.DAS.EAS.Application.Queries.GetAccountPayeSchemes;
using SFA.DAS.EAS.Application.Queries.GetUserAccounts;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.TestCommon.DependencyResolution;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Messaging;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.TestCommon.ScenarioCommonSteps
{
    public class AccountSteps
    {
        private IContainer _container;
        private Mock<IMessagePublisher> _messagePublisher;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;

        private string _externalUserId;
        private Mock<IValidator<GetAccountPayeSchemesQuery>> _validator;
        private Mock<IEventsApi> _eventsApi;

        public AccountSteps()
        {
            _messagePublisher = new Mock<IMessagePublisher>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            _eventsApi = new Mock<IEventsApi>();
            _validator = new Mock<IValidator<GetAccountPayeSchemesQuery>>();

            _validator.Setup(x => x.ValidateAsync(It.IsAny<GetAccountPayeSchemesQuery>()))
                .ReturnsAsync(new ValidationResult());

            _container = IoC.CreateContainer(_messagePublisher, _owinWrapper, _cookieService, _eventsApi);

            _container.Inject(_validator.Object);
        }
        public static void SetAccountIdForUser(IMediator mediator, ScenarioContext scenarioContext)
        {
            var accountOwnerId = scenarioContext["AccountOwnerUserId"].ToString();
            var getUserAccountsQueryResponse = mediator.SendAsync(new GetUserAccountsQuery { UserRef = accountOwnerId }).Result;

            var account = getUserAccountsQueryResponse.Accounts.AccountList.FirstOrDefault();
            scenarioContext["AccountId"] = account.Id;
            scenarioContext["HashedAccountId"] = account.HashedId;
        }

        public void CreateAccountWithOwner(EmployerAccountOrchestrator orchestrator, IMediator mediator, Mock<IOwinWrapper> owinWrapper, HomeOrchestrator homeOrchestrator)
        {
            var accountOwnerUserId = Guid.NewGuid().ToString();
            ScenarioContext.Current["AccountOwnerUserId"] = accountOwnerUserId;

            var signInUserModel = new UserViewModel
            {
                UserId = accountOwnerUserId,
                Email = "accountowner@test.com" + Guid.NewGuid().ToString().Substring(0, 6),
                FirstName = "Test",
                LastName = "Tester"
            };
            var userCreationSteps = new UserSteps();
            userCreationSteps.UpsertUser(signInUserModel);

            var user = userCreationSteps.GetExistingUserAccount();

            CreateDasAccount(user, _container.GetInstance<EmployerAccountOrchestrator>());
        }

        public static void CreateDasAccount(UserViewModel userView, EmployerAccountOrchestrator orchestrator)
        {

            orchestrator.CreateAccount(new CreateAccountViewModel
            {
                UserId = userView.UserId,
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
