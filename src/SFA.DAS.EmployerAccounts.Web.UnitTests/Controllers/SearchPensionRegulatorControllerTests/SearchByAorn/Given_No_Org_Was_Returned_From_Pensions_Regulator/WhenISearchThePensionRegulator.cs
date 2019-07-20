using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
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
       
        [SetUp]
        public void Setup()
        {                   
            var orchestrator = new Mock<SearchPensionRegulatorOrchestrator>();

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

            var userAornLockOrchestrator = new Mock<UserAornLockOrchestrator>();

            userAornLockOrchestrator.Setup(x => x.GetUserAornLockStatus(It.IsAny<string>())).ReturnsAsync(
                new OrchestratorResponse<UserAornPayeStatus>
                {
                    Data = new UserAornPayeStatus
                    {
                        RemainingLock = 0
                    }
                });

            _controller = new SearchPensionRegulatorController(
                Mock.Of<IAuthenticationService>(),
                orchestrator.Object,
                userAornLockOrchestrator.Object,
                Mock.Of<IMultiVariantTestingService>(),
                Mock.Of<ICookieStorageService<FlashMessageViewModel>>(),
                Mock.Of<IMediator>());
        }

        [Test]
        public async Task ThenTheSearchUsingAornPageIsDisplayed()
        {
            var response = await _controller.SearchPensionRegulatorByAorn(new SearchPensionRegulatorByAornViewModel { Aorn = ExpectedAorn, PayeRef = ExpectedPayeRef });
            var viewResponse = (ViewResult)response;

            Assert.AreEqual(ControllerConstants.SearchUsingAornViewName, viewResponse.ViewName);
        }
    }
}
