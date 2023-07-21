using MediatR;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.SearchPensionRegulatorControllerTests.SearchByAorn;

[TestFixture]
class WhenIDontProvideAnAorn
{
    private SearchPensionRegulatorController _controller;
        
    [SetUp]
    public void Setup()
    {
        _controller = new SearchPensionRegulatorController(
            Mock.Of<SearchPensionRegulatorOrchestrator>(),
            Mock.Of<ICookieStorageService<FlashMessageViewModel>>(),
            Mock.Of<IMediator>(),
            Mock.Of<ICookieStorageService<HashedAccountIdModel>>());
    }

    [TestCase("")]
    [TestCase(null)]
    public async Task ThenAnErrorIsDisplayed(string aorn)
    {
        var response = await _controller.SearchPensionRegulatorByAorn(new SearchPensionRegulatorByAornViewModel { Aorn = aorn, PayeRef = "000/EDDEFDS" });
        var viewResponse = (ViewResult)response;

        Assert.AreEqual(ControllerConstants.SearchUsingAornViewName, viewResponse.ViewName);
        var viewModel = viewResponse.Model as SearchPensionRegulatorByAornViewModel;
        Assert.AreEqual("Enter your Accounts Office reference in the correct format", viewModel.AornError);
    }
}