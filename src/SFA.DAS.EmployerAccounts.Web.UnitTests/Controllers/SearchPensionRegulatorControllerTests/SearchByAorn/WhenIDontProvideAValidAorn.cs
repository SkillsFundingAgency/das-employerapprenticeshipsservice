using MediatR;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.SearchPensionRegulatorControllerTests.SearchByAorn;

[TestFixture]
class WhenIDontProvideAValidAorn
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

    [Test]
    public async Task ThenAnErrorIsDisplayed()
    {
        var response = await _controller.SearchPensionRegulatorByAorn(new SearchPensionRegulatorByAornViewModel { Aorn = "SDCXDD", PayeRef = "000/EDDEFDS" });
        var viewResponse = (ViewResult)response;

        Assert.AreEqual(ControllerConstants.SearchUsingAornViewName, viewResponse.ViewName);
        var viewModel = viewResponse.Model as SearchPensionRegulatorByAornViewModel;
        Assert.AreEqual("Enter your Accounts Office reference in the correct format", viewModel.AornError);
    }
}