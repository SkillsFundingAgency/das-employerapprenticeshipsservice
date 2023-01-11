using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerTeamControllerTests
{
    public class WhenNoPayeScheme
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
        public void ThenTheAddPayeViewIsReturnedAtRow1Panel1()
        {
            // Arrange
            var model = new AccountDashboardViewModel
            {
                PayeSchemeCount = 0
            };

            //Act
            var result = _controller.Row1Panel1(model) as PartialViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AddPAYE", (result.Model as dynamic).ViewName);
        }
    }
}
