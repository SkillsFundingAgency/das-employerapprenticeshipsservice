using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.SearchPensionRegulatorControllerTests.SearchByPaye.Given_Multiple_Org_Was_Returned_From_Pensions_Regulator
{
    [TestFixture]
    class WhenISearchThePensionRegulator
    {
        private SearchPensionRegulatorController _controller;    
       
        [SetUp]
        public void Setup()
        {                   
            var orchestrator = new Mock<SearchPensionRegulatorOrchestrator>();

            orchestrator
                .Setup(x => x.SearchPensionRegulator(It.IsAny<string>()))
                .ReturnsAsync(
                    new OrchestratorResponse<SearchPensionRegulatorResultsViewModel>
                    {
                        Data = new SearchPensionRegulatorResultsViewModel
                        {
                            Results = new List<PensionRegulatorDetailsViewModel>
                            {
                                new PensionRegulatorDetailsViewModel(),
                                new PensionRegulatorDetailsViewModel()
                            }
                        }
                    });

            orchestrator.Setup(x => x.GetCookieData())
                .Returns(
                    new EmployerAccountData
                    {
                        EmployerAccountPayeRefData = new EmployerAccountPayeRefData
                        {
                            PayeReference = "PayeRef"
                        }
                    });

            _controller = new SearchPensionRegulatorController(
                Mock.Of<IAuthenticationService>(),
                orchestrator.Object,
                Mock.Of<IMultiVariantTestingService>(),
                Mock.Of<ICookieStorageService<FlashMessageViewModel>>(),
                Mock.Of<IMediator>(),
                Mock.Of<ICookieStorageService<HashedAccountIdModel>>());
        }

        [Test]
        public async Task ThenThePensionRegulatorResultsPageIsDisplayed()
        {
            var response = await _controller.SearchPensionRegulator(It.IsAny<string>());
            var viewResponse = (ViewResult) response;

            Assert.AreEqual(ControllerConstants.SearchPensionRegulatorResultsViewName, viewResponse.ViewName);
        }
    }
}
