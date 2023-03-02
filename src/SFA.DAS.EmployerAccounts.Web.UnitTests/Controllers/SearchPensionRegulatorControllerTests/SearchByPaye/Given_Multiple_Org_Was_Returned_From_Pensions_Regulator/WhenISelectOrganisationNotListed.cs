using MediatR;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.SearchPensionRegulatorControllerTests.SearchByPaye.Given_Multiple_Org_Was_Returned_From_Pensions_Regulator;

[TestFixture]
class WhenISelectOrganisationNotListed
{
    private SearchPensionRegulatorController _controller;    
       
    [SetUp]
    public void Setup()
    {                   
        var orchestrator = new Mock<SearchPensionRegulatorOrchestrator>();
          
        _controller = new SearchPensionRegulatorController(
            orchestrator.Object,            
            Mock.Of<ICookieStorageService<FlashMessageViewModel>>(),
            Mock.Of<IMediator>(),
            Mock.Of<ICookieStorageService<HashedAccountIdModel>>());
    }

    [Test]
    public void ThenTheSearchOrganisationPageIsDisplayed()
    {
        var viewModel = new SearchPensionRegulatorResultsViewModel
        {
            Results = new List<PensionRegulatorDetailsViewModel>
            {
                new PensionRegulatorDetailsViewModel { ReferenceNumber = 1 },
                new PensionRegulatorDetailsViewModel { ReferenceNumber = 2 }
            },
            SelectedOrganisation = 0
        };

        var response = _controller.SearchPensionRegulator(It.IsAny<string>(), viewModel).Result;
        var redirectResponse = (RedirectToActionResult) response;

        Assert.AreEqual(ControllerConstants.SearchForOrganisationActionName, redirectResponse.ActionName);
        Assert.AreEqual(ControllerConstants.SearchOrganisationControllerName, redirectResponse.ControllerName);
    }
}