using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using BoDi;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Application.Commands.AddPayeToAccount;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.PAYE;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.TestCommon.DependencyResolution;
using SFA.DAS.EAS.TestCommon.Extensions;
using SFA.DAS.EAS.Web.Authorization;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Filters;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Types;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using TechTalk.SpecFlow;
using RedirectToRouteResult = System.Web.Http.Results.RedirectToRouteResult;

namespace SFA.DAS.EAS.TestCommon.Steps
{
    [Binding]
    public class PayeSchemeSteps
    {
        private const string PayeSchemeAddedEventName = "PayeSchemeAddedEvent";
        private const string PayeSchemeRemovedEventName = "PayeSchemeRemovedEvent";

        private readonly IObjectContainer _objectContainer;
        private readonly ObjectContext _objectContext;

        private readonly EmployerAccountPayeOrchestrator _eapOrchestrator;
        private readonly EmployerAccountPayeController _controller;

        private ActionResult _actionResult;
        private readonly Mock<IAuthenticationService> _authenticationServiceMock;
        private readonly Mock<IEventsApi> _eventsApi;

        public PayeSchemeSteps(IObjectContainer objectContainer, ObjectContext objectContext)
        {
            _objectContainer = objectContainer;
            _objectContext = objectContext;

            var messagePublisher = new Mock<IMessagePublisher>();
            _authenticationServiceMock = new Mock<IAuthenticationService>();

            var cookieServiceEmployerAccountData = new Mock<ICookieStorageService<EmployerAccountData>>();
            var cookieServiceFlashMessageViewModel = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            var commitmentsApi = new Mock<IEmployerCommitmentApi>();

            var authorizationService = new Mock<IAuthorizationService>();

            _objectContainer = objectContainer;
            _objectContainer.RegisterInstanceAs(messagePublisher.Object);
            _objectContainer.RegisterInstanceAs(_authenticationServiceMock.Object);
            _objectContainer.RegisterInstanceAs(cookieServiceEmployerAccountData.Object);
            _objectContainer.RegisterInstanceAs(cookieServiceFlashMessageViewModel.Object);
            _objectContainer.RegisterInstanceAs(commitmentsApi.Object);
            _objectContainer.RegisterInstanceAs(authorizationService.Object);

            _eapOrchestrator = new EmployerAccountPayeOrchestrator(_objectContainer.Resolve<IMediator>(),
                _objectContainer.Resolve<ILog>(), _objectContainer.Resolve<ICookieStorageService<EmployerAccountData>>()
                , _objectContainer.Resolve<EmployerApprenticeshipsServiceConfiguration>());

            _controller = new EmployerAccountPayeController(_objectContainer.Resolve<IAuthenticationService>(),
                _eapOrchestrator,
                _objectContainer.Resolve<IAuthorizationService>(),
                _objectContainer.Resolve<IMultiVariantTestingService>(),
                _objectContainer.Resolve<ICookieStorageService<FlashMessageViewModel>>());
        }

        [Given(@"user ([^ ]*) adds paye scheme ""([^ ]*)"" to account ([^ ]*)")]
        [When(@"user ([^ ]*) adds paye scheme ""([^ ]*)"" to account ([^ ]*)")]
        public async Task WhenUserUAddsPayeSchemePToAccountA(string username, string payeSchemeRef, string accountName)
        {
            var user = _objectContext.Users[username];
            var account = _objectContext.Accounts[accountName];

            var addPayeSchemeVm = new AddNewPayeSchemeViewModel
            {
                HashedAccountId = account.HashedId,
                AccessToken = "AccessToken",
                PayeName = "Name",
                PayeScheme = payeSchemeRef,
                UserId = user.Id,
                PublicHashedAccountId = account.PublicHashedId,
                RefreshToken = "RefreshToken"
            };

            user.SetMockAuthenticationServiceForUser(_authenticationServiceMock);

            _actionResult = await _controller.ConfirmPayeScheme(account.HashedId, addPayeSchemeVm);
        }

        [When(@"user ([^ ]*) removes paye scheme ""(.*)"" from account ([^ ]*)")]
        public async Task WhenUserDaveRemovesPayeSchemeFromAccountA(string username, string payeSchemeRef, string accountName)
        {
            var user = _objectContext.Users[username];
            var account = _objectContext.Accounts[accountName];

            var removePayeSchemeVm = new RemovePayeSchemeViewModel()
            {
                HashedAccountId = account.HashedId,
                UserId = user.Id.ToString(),
                PublicHashedAccountId = account.PublicHashedId,
                PayeRef = payeSchemeRef,
                RemoveScheme = 2
            };

            user.SetMockAuthenticationServiceForUser(_authenticationServiceMock);
            _actionResult = await _controller.RemovePaye(account.HashedId, removePayeSchemeVm);
        }


        [Then(@"The an active PAYE scheme ""(.*)"" is added to account ([^ ] *)")]
        public async Task ThenAnActivePayeSchemePIsAddedToAccountA(string payeSchemeRef, string accountName)
        {
            var account = _objectContext.Accounts[accountName];

            var repo = _objectContainer.Resolve<IPayeRepository>();
            var payeScheme = await repo.GetPayeForAccountByRef(account.HashedId, payeSchemeRef);

            Assert.IsNotNull(payeScheme);
            Assert.IsNull(payeScheme.RemovedDate);
        }

        [Then(@"The an active PAYE scheme ""(.*)"" is removed from account ([^ ] *)")]
        public async Task ThenTheAnActivePAYESchemeIsRemovedFromAccountA(string payeSchemeRef, string accountName)
        {
            var account = _objectContext.Accounts[accountName];

            var repo = _objectContainer.Resolve<IPayeRepository>();
            var payeScheme = await repo.GetPayeForAccountByRef(account.HashedId, payeSchemeRef);

            Assert.IsNotNull(payeScheme.RemovedDate);
        }


        [Then(@"The user is redirected to the next steps view")]
        public void ThenTheUserIsRedirectedToTheNextStepsView()
        {
            var redirect = (System.Web.Mvc.RedirectToRouteResult)_actionResult;

            Assert.IsNotNull(redirect);
            Assert.AreEqual(ControllerConstants.NextStepsActionName, redirect.RouteValues["action"]);
            Assert.AreEqual(ControllerConstants.EmployerAccountPayeControllerName, redirect.RouteValues["controller"]);
        }

        [Then(@"The user is redirected to the paye index view")]
        public void ThenTheUserIsRedirectedToThePayeIndexView()
        {
            var redirect = (System.Web.Mvc.RedirectToRouteResult)_actionResult;

            Assert.IsNotNull(redirect);
            Assert.AreEqual(ControllerConstants.IndexActionName, redirect.RouteValues["action"]);
            Assert.AreEqual(ControllerConstants.EmployerAccountPayeControllerName, redirect.RouteValues["controller"]);
        }

        [Then(@"A Paye Scheme Added event is raised")]
        public void ThenAPayeSchemeAddedEventIsRaised()
        {
            _objectContext.EventsApiMock.Verify(e =>
                e.CreateGenericEvent(It.Is<GenericEvent>(ev => ev.Type == PayeSchemeAddedEventName)));
        }

        [Then(@"A Paye Scheme Removed event is raised")]
        public void ThenAPayeSchemeRemovedEventIsRaised()
        {
            _objectContext.EventsApiMock.Verify(e =>
                e.CreateGenericEvent(It.Is<GenericEvent>(ev => ev.Type == PayeSchemeRemovedEventName)));
        }
    }
}