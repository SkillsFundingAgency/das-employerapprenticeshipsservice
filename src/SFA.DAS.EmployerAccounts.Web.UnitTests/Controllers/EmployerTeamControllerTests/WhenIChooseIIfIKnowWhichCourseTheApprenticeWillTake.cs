using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerTeamControllerTests;

public class WhenIChooseIIfIKnowWhichCourseTheApprenticeWillTake
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
            _mockEmployerTeamOrchestrator.Object,
            Mock.Of<IHttpContextAccessor>());
    }

    [Test]
    public void IfIChooseYesIContinueTheJourney()
    {
        //Act
        var result = _controller.TriageWhichCourseYourApprenticeWillTake(new TriageViewModel { TriageOption = TriageOptions.Yes }) as RedirectToRouteResult;

        //Assert
        Assert.AreEqual(ControllerConstants.TriageHaveYouChosenATrainingProviderActionName, result.RouteValues["Action"]);
    }

    [Test]
    public void IfIChooseNoICannotSetupAnApprentice()
    {
        //Act
        var result = _controller.TriageWhichCourseYourApprenticeWillTake(new TriageViewModel { TriageOption = TriageOptions.No }) as RedirectToRouteResult;

        //Assert
        Assert.AreEqual(ControllerConstants.TriageYouCannotSetupAnApprenticeshipYetCourseProviderActionName, result.RouteValues["Action"]);
    }
}