using MediatR;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.AmendOrganisation.Given_Multiple_Orgs_Were_Returned_From_Pensions_Regulator_Via_GG;

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
            EmployerAccountOrganisationData = new EmployerAccountOrganisationData { OrganisationType = OrganisationType.PensionsRegulator, PensionsRegulatorReturnedMultipleResults = true },
            EmployerAccountPayeRefData = new EmployerAccountPayeRefData { AORN = "" }
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
    public void ThenTheGovernmentGatewayPensionRegulatorChooseOrganisationPageIsDisplayed()
    {
        var response = _employerAccountController.AmendOrganisation();
        var redirectResponse = (RedirectToActionResult)response;

        Assert.AreEqual(ControllerConstants.SearchPensionRegulatorActionName, redirectResponse.ActionName);
        Assert.AreEqual(ControllerConstants.SearchPensionRegulatorControllerName, redirectResponse.ControllerName);
    }
}