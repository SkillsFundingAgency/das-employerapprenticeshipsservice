using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Queries.GetUserAornLock;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.EmployerAccounts.Queries.UpdateUserAornLock;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeInUse;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.SearchPensionRegulatorControllerTests.SearchByAorn.Given_No_Org_Was_Returned_From_Pensions_Regulator
{
    [TestFixture]
    class WhenISearchThePensionRegulator
    {
        private SearchPensionRegulatorController _controller;
        private const string ExpectedAorn = "aorn";
        private const string ExpectedPayeRef = "payeref";
        private readonly string _expectedId = Guid.NewGuid().ToString();

        [SetUp]
        public void Setup()
        {                   
            var orchestrator = new Mock<SearchPensionRegulatorOrchestrator>();
            var mediator = new Mock<IMediator>();
            var owinWrapper = new Mock<IAuthenticationService>();
            owinWrapper.Setup(x => x.GetClaimValue(ControllerConstants.UserRefClaimKeyName)).Returns(_expectedId);

            orchestrator
                .Setup(x => x.GetOrganisationsByAorn(ExpectedAorn, ExpectedPayeRef))
                .ReturnsAsync(
                    new OrchestratorResponse<SearchPensionRegulatorResultsViewModel>
                    {
                        Data = new SearchPensionRegulatorResultsViewModel
                        {
                            Results = new List<PensionRegulatorDetailsViewModel>()
                        }
                    });

            mediator.Setup(x => x.SendAsync(new UpdateUserAornLockRequest()));
			mediator.Setup(x => x.SendAsync(It.IsAny<GetPayeSchemeInUseQuery>())).ReturnsAsync(new GetPayeSchemeInUseResponse());
            mediator.Setup(x => x.SendAsync(It.IsAny<GetUserAornLockRequest>())).ReturnsAsync(
                new GetUserAornLockResponse
                {
                    UserAornStatus = new UserAornPayeStatus
                    {
                        RemainingLock = 0
                    }
                });

            _controller = new SearchPensionRegulatorController(
                owinWrapper.Object,
                orchestrator.Object,           
                Mock.Of<IMultiVariantTestingService>(),
                Mock.Of<ICookieStorageService<FlashMessageViewModel>>(),
                mediator.Object,
                Mock.Of<ICookieStorageService<HashedAccountIdModel>>());
        }

        [Test]
        public async Task ThenTheSearchUsingAornPageIsDisplayed()
        {
            var response = await _controller.SearchPensionRegulatorByAorn(new SearchPensionRegulatorByAornViewModel { Aorn = ExpectedAorn, PayeRef = ExpectedPayeRef });
            var viewResponse = (Microsoft.AspNetCore.Mvc.ViewResult) response;

            Assert.AreEqual(ControllerConstants.SearchUsingAornViewName, viewResponse.ViewName);
        }
    }
}
