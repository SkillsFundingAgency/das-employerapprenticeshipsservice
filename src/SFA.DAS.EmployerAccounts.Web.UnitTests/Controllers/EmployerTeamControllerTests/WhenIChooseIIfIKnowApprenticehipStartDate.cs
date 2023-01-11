﻿using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerTeamControllerTests
{
    public class WhenIChooseIIfIKnowApprenticehipStartDate
    {
        private EmployerTeamController _controller;

        private Mock<IAuthenticationService> mockAuthenticationService;
        private Mock<IMultiVariantTestingService> mockMultiVariantTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> mockCookieStorageService;
        private Mock<EmployerTeamOrchestrator> mockEmployerTeamOrchestrator;

        [SetUp]
        public void Arrange()
        {
            mockAuthenticationService = new Mock<IAuthenticationService>();
            mockMultiVariantTestingService = new Mock<IMultiVariantTestingService>();
            mockCookieStorageService = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            mockEmployerTeamOrchestrator = new Mock<EmployerTeamOrchestrator>();

            _controller = new EmployerTeamController(
                mockAuthenticationService.Object,
                mockMultiVariantTestingService.Object,
                mockCookieStorageService.Object,
                mockEmployerTeamOrchestrator.Object);
        }

        [Test]
        public void IfIChooseYesIContinueTheJourney()
        {
            //Act
            var result = _controller.TriageWillApprenticeshipTrainingStart(new TriageViewModel { TriageOption = TriageOptions.Yes }) as RedirectToRouteResult;

            //Assert
            Assert.AreEqual(ControllerConstants.TriageApprenticeForExistingEmployeeActionName, result.RouteValues["Action"]);
        }

        [Test]
        public void IfIChooseNoICannotSetupAnApprentice()
        {
            //Act
            var result = _controller.TriageWillApprenticeshipTrainingStart(new TriageViewModel { TriageOption = TriageOptions.No }) as RedirectToRouteResult;

            //Assert
            Assert.AreEqual(ControllerConstants.TriageYouCannotSetupAnApprenticeshipYetStartDateActionName, result.RouteValues["Action"]);
        }

        [Test]
        public void IfIChooseDontKnowICannotSetupAnApprentice()
        {
            //Act
            var result = _controller.TriageWillApprenticeshipTrainingStart(new TriageViewModel { TriageOption = TriageOptions.Unknown }) as RedirectToRouteResult;

            //Assert
            Assert.AreEqual(ControllerConstants.TriageYouCannotSetupAnApprenticeshipYetApproximateStartDateActionName, result.RouteValues["Action"]);
        }
    }
}
