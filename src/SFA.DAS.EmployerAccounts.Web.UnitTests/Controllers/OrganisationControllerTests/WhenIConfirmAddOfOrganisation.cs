using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.OrganisationControllerTests;

/// <summary>
/// AML-2459: Move to EmployerAccounts site tests
/// </summary>
public class WhenIConfirmAddOfOrganisation
{
    private OrganisationController _controller;
    private Mock<OrganisationOrchestrator> _orchestrator;
    private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

    private const string TestHashedAgreementId = "DEF456";

    [SetUp]
    public void Arrange()
    {
        _orchestrator = new Mock<OrganisationOrchestrator>();
        _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

        _orchestrator.Setup(x => x.CreateLegalEntity(It.IsAny<CreateNewLegalEntityViewModel>()))
            .ReturnsAsync(new OrchestratorResponse<EmployerAgreementViewModel>
            {
                Status = HttpStatusCode.OK,
                Data = new EmployerAgreementViewModel
                {
                    EmployerAgreement = new EmployerAgreementView
                    {
                        HashedAgreementId = TestHashedAgreementId
                    }
                }
            });

        _controller = new OrganisationController(
            _orchestrator.Object,           
            _flashMessage.Object,
            Mock.Of<IHttpContextAccessor>());
    }

    [Test]
    public async Task ThenIAmRedirectedToNextStepsViewIfSuccessful()
    {
        //Act
        var result = await _controller.Confirm("", "", "", "", null, "", OrganisationType.Other, 1, null, false) as RedirectToRouteResult;

        //Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("OrganisationAddedNextSteps", result.RouteValues["Action"]);
    }

    [Test]
    public async Task ThenIAmRedirectedToNextStepsNewSearchIfTheNewSearchFlagIsSet()
    {
        //Act
        var result = await _controller.Confirm("", "", "", "", null, "", OrganisationType.Other, 1, null, true) as RedirectToRouteResult;

        //Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("OrganisationAddedNextStepsSearch", result.RouteValues["Action"]);
    }

    [Test]
    public async Task ThenIAmSuppliedTheHashedAgreementIdForANewSearch()
    {
        //Act
        var result = await _controller.Confirm("", "", "", "", null, "", OrganisationType.Other, 1, null, true) as RedirectToRouteResult;

        //Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(TestHashedAgreementId, result.RouteValues["HashedAgreementId"]);
    }
}