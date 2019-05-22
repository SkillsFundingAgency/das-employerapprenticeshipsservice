using System.Web.Mvc;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Portal.Client;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerTeamControllerTests
{
    [TestFixture]
    class WhenIViewPanel2OfHomepage
    {
        private EmployerTeamController _controller;
        private Mock<EmployerTeamOrchestrator> _orchestrator;
        private Mock<INextActionPanelViewHelper> _homepagePanelViewHelper;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IAuthorizationService> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
        private Mock<IPortalClient> _portalClientMock;
        private Mock<IHashingService> _hashingServiceMock;

        [SetUp]
        public void Arrange()
        {

            _owinWrapper = new Mock<IAuthenticationService>();
            _featureToggle = new Mock<IAuthorizationService>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            _homepagePanelViewHelper = new Mock<INextActionPanelViewHelper>();
            _orchestrator = new Mock<EmployerTeamOrchestrator>(new Mock<IMediator>().Object, Mock.Of<ICurrentDateTime>());
            _hashingServiceMock = new Mock<IHashingService>();
            _portalClientMock = new Mock<IPortalClient>();

            _controller = new EmployerTeamController(
                _owinWrapper.Object,
                _featureToggle.Object,
                _userViewTestingService.Object,
                _flashMessage.Object,
                _orchestrator.Object,
                _homepagePanelViewHelper.Object,
                _portalClientMock.Object,
                _hashingServiceMock.Object);
        }

        [Test, Category("UnitTest")]
        public void WhenNoPayeSchemeAdded_ShouldNotBeAbleToSetProviderPermissions()
        {
            //Arrange
            var model = new AccountDashboardViewModel
            {
                PayeSchemeCount = 0
            };

            //Act
            var result = _controller.Row1Panel2(model) as PartialViewResult;
            var viewModel = result?.Model as PanelViewModel<AccountDashboardViewModel>;

            //Assert
            Assert.IsNotNull(model);
            viewModel.ViewName.Should().Be("ProviderPermissionsDenied");
        }

        [Test, Category("UnitTest")]
        public void WhenPayeSchemeExists_ShouldBeAbleToSetProviderPermissions()
        {
            //Arrange
            var model = new AccountDashboardViewModel
            {
                PayeSchemeCount = 1
            };

            //Act
            var result = _controller.Row1Panel2(model) as PartialViewResult;
            var viewModel = result?.Model as PanelViewModel<AccountDashboardViewModel>;

            //Assert
            Assert.IsNotNull(model);
            viewModel.ViewName.Should().Be("ProviderPermissions");
        }
    }
}
