using System.Collections.Generic;
using System.Web.Mvc;
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
    class WhenIDontSelectAnOrganisation
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
        public void ThenThePensionRegulatorResultsPageIsDisplayed()
        {
            var viewModel = new SearchPensionRegulatorResultsViewModel
            {
                Results = new List<PensionRegulatorDetailsViewModel>
                {
                    new PensionRegulatorDetailsViewModel { ReferenceNumber = 1 },
                    new PensionRegulatorDetailsViewModel { ReferenceNumber = 2 }
                }             
            };

            var response = _controller.SearchPensionRegulator(It.IsAny<string>(), viewModel).Result;
            var viewResponse = (Microsoft.AspNetCore.Mvc.ViewResult) response;

            Assert.AreEqual(ControllerConstants.SearchPensionRegulatorResultsViewName, viewResponse.ViewName);
            Assert.AreEqual(true, viewResponse.ViewBag.InError);
        }
    }
}
