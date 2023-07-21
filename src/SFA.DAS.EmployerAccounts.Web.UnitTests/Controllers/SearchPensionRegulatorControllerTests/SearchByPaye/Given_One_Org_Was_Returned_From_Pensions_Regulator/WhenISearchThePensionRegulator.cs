using MediatR;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.SearchPensionRegulatorControllerTests.SearchByPaye.Given_One_Org_Was_Returned_From_Pensions_Regulator;

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
            orchestrator.Object,
            Mock.Of<ICookieStorageService<FlashMessageViewModel>>(),
            Mock.Of<IMediator>(),
            Mock.Of<ICookieStorageService<HashedAccountIdModel>>());
    }

    [Test]
    public async Task ThenTheCheckYourDetailsPageIsDisplayed()
    {
        var response = await _controller.SearchPensionRegulator(It.IsAny<string>());
        var redirectResponse = (RedirectToActionResult)response;

        Assert.AreEqual(ControllerConstants.SummaryActionName, redirectResponse.ActionName);
        Assert.AreEqual(ControllerConstants.EmployerAccountControllerName, redirectResponse.ControllerName);
    }
}