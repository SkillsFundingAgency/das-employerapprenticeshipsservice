using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Commands.PayeRefData;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeInUse;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.SearchPensionRegulatorControllerTests.SearchByAorn.Given_Multiple_Org_Was_Returned_From_Pensions_Regulator
{
    [TestFixture]
    class WhenISearchThePensionRegulator
    {
        private const string ExpectedAorn = "0123456789ABC";
        private const string ExpectedPayeRef = "000/1234567";
        private SearchPensionRegulatorController _controller;
        private SearchPensionRegulatorResultsViewModel _expectedData;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Setup()
        {
            _expectedData = new SearchPensionRegulatorResultsViewModel { Results = new List<PensionRegulatorDetailsViewModel> { new PensionRegulatorDetailsViewModel(), new PensionRegulatorDetailsViewModel() } };
            var orchestrator = new Mock<SearchPensionRegulatorOrchestrator>();

            orchestrator
                .Setup(x => x.GetOrganisationsByAorn(ExpectedAorn, ExpectedPayeRef))
                .ReturnsAsync(
                    new OrchestratorResponse<SearchPensionRegulatorResultsViewModel>
                    {
                        Data = _expectedData
                    });

            orchestrator.Setup(x => x.GetCookieData())
                .Returns(
                    new EmployerAccountData
                    {
                        EmployerAccountPayeRefData = new EmployerAccountPayeRefData
                        {
                            PayeReference = "000/1234567"
                        }
                    });

            _mediator = new Mock<IMediator>();

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetPayeSchemeInUseQuery>())).ReturnsAsync(new GetPayeSchemeInUseResponse());

            _controller = new SearchPensionRegulatorController(
                Mock.Of<IAuthenticationService>(),
                orchestrator.Object,
                Mock.Of<IMultiVariantTestingService>(),
                Mock.Of<ICookieStorageService<FlashMessageViewModel>>(),
                _mediator.Object);
        }

        [Test]
        public async Task ThenThePayeDetailsAreSaved()
        {
            await _controller.SearchPensionRegulatorByAorn(new SearchPensionRegulatorByAornViewModel { Aorn = ExpectedAorn, PayeRef = ExpectedPayeRef });

            _mediator.Verify(x => x.SendAsync(It.Is<SavePayeRefData>(y => y.PayeRefData.AORN == ExpectedAorn && y.PayeRefData.PayeReference == ExpectedPayeRef)));
        }

        [Test]
        public async Task ThenThePensionRegulatorResultsPageIsDisplayed()
        {
            var response = await _controller.SearchPensionRegulatorByAorn(new SearchPensionRegulatorByAornViewModel { Aorn = ExpectedAorn, PayeRef = ExpectedPayeRef });
            var viewResponse = (ViewResult) response;

            Assert.AreEqual(ControllerConstants.SearchPensionRegulatorResultsViewName, viewResponse.ViewName);
            Assert.AreSame(_expectedData, viewResponse.Model);
        }
    }
}
