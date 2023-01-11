using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Web.Models;
using Microsoft.AspNetCore.Mvc.Routing;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests
{
    public class WhenIRenameAnAccount : ControllerTestBase
    {
        private EmployerAccountController _employerAccountController;
        private Mock<EmployerAccountOrchestrator> _orchestrator;
        private Mock<IAuthenticationService> _owinWrapper;     
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
        private const string ExpectedRedirectUrl = "http://redirect.local.test";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(ExpectedRedirectUrl);

            _orchestrator = new Mock<EmployerAccountOrchestrator>();

            _owinWrapper = new Mock<IAuthenticationService>();        
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            var logger = new Mock<ILog>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _orchestrator.Setup(x => x.RenameEmployerAccount(It.IsAny<RenameEmployerAccountViewModel>(), It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<RenameEmployerAccountViewModel>
                {
                    Status = HttpStatusCode.OK,
                    Data = new RenameEmployerAccountViewModel()
                });

            _employerAccountController = new EmployerAccountController(_owinWrapper.Object,
                _orchestrator.Object,
                _userViewTestingService.Object,
                logger.Object,
                _flashMessage.Object,
                Mock.Of<IMediator>(),
                Mock.Of<ICookieStorageService<ReturnUrlModel>>(),
                Mock.Of<ICookieStorageService<HashedAccountIdModel>>())
            {
                ControllerContext = _controllerContext.Object,
                Url = new UrlHelper(new RequestContext(_httpContext.Object, new RouteData()), _routes)
            };
        }

        [Test]
        public async Task ThenTheAccountIsRenamed()
        {
            //Arrange
            var model = new RenameEmployerAccountViewModel
            {
                CurrentName = "Test Account",
                NewName = "New Account Name",
                HashedId = "ABC123"
            };

            //Act
            await _employerAccountController.RenameAccount(model);

            //Assert
            _orchestrator.Verify(x => x.RenameEmployerAccount(It.Is<RenameEmployerAccountViewModel>(r =>
                r.CurrentName == "Test Account"
                && r.NewName == "New Account Name"
            ), It.IsAny<string>()));
        }

    }
}
