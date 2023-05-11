using MediatR;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.AmendOrganisation.Given_One_Organisation_Was_Returned_From_Pensions_Regulator;

[TestFixture]
public class WhenIAmendTheOrganisation
{
    private EmployerAccountController _employerAccountController;

    [SetUp]
    public void Setup()
    {
        var orchestrator = new Mock<EmployerAccountOrchestrator>();
        orchestrator.Setup(x => x.GetCookieData()).Returns(new EmployerAccountData
        {
            EmployerAccountOrganisationData = new EmployerAccountOrganisationData { OrganisationType = OrganisationType.PensionsRegulator, PensionsRegulatorReturnedMultipleResults = false }
        });

        _employerAccountController = new EmployerAccountController(
            orchestrator.Object,
            Mock.Of<ILogger<EmployerAccountController>>(),
            Mock.Of<ICookieStorageService<FlashMessageViewModel>>(),
            Mock.Of<IMediator>(),
            Mock.Of<ICookieStorageService<ReturnUrlModel>>(),
            Mock.Of<ICookieStorageService<HashedAccountIdModel>>(),
            Mock.Of<LinkGenerator>());
    }

    [Test]
    public void ThenTheSearchOrganisationPageIsDisplayed()
    {
        var response =  _employerAccountController.AmendOrganisation();
        var redirectResponse = (RedirectToActionResult)response;

        Assert.AreEqual(ControllerConstants.SearchForOrganisationActionName, redirectResponse.ActionName);
        Assert.AreEqual(ControllerConstants.SearchOrganisationControllerName, redirectResponse.ControllerName);
    }
}