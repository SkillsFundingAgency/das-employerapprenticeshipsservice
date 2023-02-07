using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerTeamControllerTests;

public class WhenIChooseIIfIKnowApprenticehipStartDate 
{
    private EmployerTeamController _controller;

    private Mock<ICookieStorageService<FlashMessageViewModel>> _mockCookieStorageService;
    private Mock<EmployerTeamOrchestrator> _mockEmployerTeamOrchestrator;

    [SetUp]
    public void Arrange()
    {
        _mockCookieStorageService = new Mock<ICookieStorageService<FlashMessageViewModel>>();
        _mockEmployerTeamOrchestrator = new Mock<EmployerTeamOrchestrator>();

        _controller = new EmployerTeamController(
            _mockCookieStorageService.Object,
            _mockEmployerTeamOrchestrator.Object);
    }

    [Test]
    public void IfIChooseYesIContinueTheJourney()
    {
        //Act
        var result = _controller.TriageWillApprenticeshipTrainingStart(new TriageViewModel { TriageOption = TriageOptions.Yes }) as RedirectToActionResult;

        //Assert
        Assert.AreEqual(ControllerConstants.TriageApprenticeForExistingEmployeeActionName, result.ActionName);
    }

    [Test]
    public void IfIChooseNoICannotSetupAnApprentice()
    {
        //Act
        var result = _controller.TriageWillApprenticeshipTrainingStart(new TriageViewModel { TriageOption = TriageOptions.No }) as RedirectToActionResult;

        //Assert
        Assert.AreEqual(ControllerConstants.TriageYouCannotSetupAnApprenticeshipYetStartDateActionName, result.ActionName);
    }

    [Test]
    public void IfIChooseDontKnowICannotSetupAnApprentice()
    {
        //Act
        var result = _controller.TriageWillApprenticeshipTrainingStart(new TriageViewModel { TriageOption = TriageOptions.Unknown }) as RedirectToActionResult;

        //Assert
        Assert.AreEqual(ControllerConstants.TriageYouCannotSetupAnApprenticeshipYetApproximateStartDateActionName, result.ActionName);
    }
}