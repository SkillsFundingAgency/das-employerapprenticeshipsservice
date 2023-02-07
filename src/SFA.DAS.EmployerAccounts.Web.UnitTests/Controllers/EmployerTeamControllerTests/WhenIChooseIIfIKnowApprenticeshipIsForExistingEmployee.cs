using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerTeamControllerTests;

public class WhenIChooseIIfIKnowApprenticeshipIsForExistingEmployee
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
        // Arrange
        var model = new AccountDashboardViewModel
        {
            PayeSchemeCount = 1,
            PendingAgreements = new List<PendingAgreementsViewModel> { new PendingAgreementsViewModel() }
        };

        //Act
        var result = _controller.TriageApprenticeForExistingEmployee(new TriageViewModel { TriageOption = TriageOptions.No }) as ViewResult;

        //Assert
        Assert.AreEqual(ControllerConstants.TriageSetupApprenticeshipNewEmployeeViewName, result.ViewName);
    }
}