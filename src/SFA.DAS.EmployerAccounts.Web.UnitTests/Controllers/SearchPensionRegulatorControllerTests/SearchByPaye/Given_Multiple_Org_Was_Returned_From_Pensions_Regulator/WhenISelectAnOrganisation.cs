using System.Collections.Generic;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.SearchPensionRegulatorControllerTests.SearchByPaye.Given_Multiple_Org_Was_Returned_From_Pensions_Regulator
{
    [TestFixture]
    class WhenISelectAnOrganisation
    {
        private SearchPensionRegulatorController _controller;    
       
        [SetUp]
        public void Setup()
        {                   
            var orchestrator = new Mock<SearchPensionRegulatorOrchestrator>();
          
            _controller = new SearchPensionRegulatorController(
                Mock.Of<IAuthenticationService>(),
                orchestrator.Object,              
                Mock.Of<IMultiVariantTestingService>(),
                Mock.Of<ICookieStorageService<FlashMessageViewModel>>(),
                Mock.Of<IMediator>(),
                Mock.Of<ICookieStorageService<HashedAccountIdModel>>());
        }

        [Test]
        public void ThenTheCheckYourDetailsPageIsDisplayed()
        {
            var viewModel = new SearchPensionRegulatorResultsViewModel
            {
                Results = new List<PensionRegulatorDetailsViewModel>
                {
                    new PensionRegulatorDetailsViewModel { ReferenceNumber = 1 },
                    new PensionRegulatorDetailsViewModel { ReferenceNumber = 2 }
                },
                SelectedOrganisation = 2
            };

            var response = _controller.SearchPensionRegulator(It.IsAny<string>(), viewModel).Result;
            var redirectResponse = (RedirectToRouteResult) response;

            Assert.AreEqual(ControllerConstants.SummaryActionName, redirectResponse.RouteValues["action"].ToString());
            Assert.AreEqual(ControllerConstants.EmployerAccountControllerName, redirectResponse.RouteValues["controller"].ToString());
        }
    }
}
