using MediatR;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeInUse;
using SFA.DAS.EmployerAccounts.Queries.GetUserAornLock;
using SFA.DAS.EmployerAccounts.Queries.UpdateUserAornLock;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.SearchPensionRegulatorControllerTests.SearchByAorn.Given_No_Org_Was_Returned_From_Pensions_Regulator;

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

        mediator.Setup(x => x.Send(new UpdateUserAornLockRequest(), It.IsAny<CancellationToken>()));
        mediator.Setup(x => x.Send(It.IsAny<GetPayeSchemeInUseQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetPayeSchemeInUseResponse());
        mediator.Setup(x => x.Send(It.IsAny<GetUserAornLockRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(
            new GetUserAornLockResponse
            {
                UserAornStatus = new UserAornPayeStatus
                {
                    RemainingLock = 0
                }
            });

        _controller = new SearchPensionRegulatorController(
            orchestrator.Object,           
            Mock.Of<ICookieStorageService<FlashMessageViewModel>>(),
            mediator.Object,
            Mock.Of<ICookieStorageService<HashedAccountIdModel>>());
    }

    [Test]
    public async Task ThenTheSearchUsingAornPageIsDisplayed()
    {
        var response = await _controller.SearchPensionRegulatorByAorn(new SearchPensionRegulatorByAornViewModel { Aorn = ExpectedAorn, PayeRef = ExpectedPayeRef });
        var viewResponse = (ViewResult) response;

        Assert.AreEqual(ControllerConstants.SearchUsingAornViewName, viewResponse.ViewName);
    }
}