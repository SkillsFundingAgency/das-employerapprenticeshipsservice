using System.Diagnostics.CodeAnalysis;
using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.GatewayInform
{
    [ExcludeFromCodeCoverage]
    public class When_I_Call_GatewayInform_Without_HashedAccountId : ControllerTestBase
    {
        private EmployerAccountController _employerAccountController;
        private Mock<EmployerAccountOrchestrator> _orchestrator;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<ICookieStorageService<HashedAccountIdModel>> _mockAccountCookieStorage;
        private string _cookieKeyName;

        [SetUp]
        public void Arrange()
        {
            base.Arrange();

            _cookieKeyName = typeof(HashedAccountIdModel).FullName;

            _orchestrator = new Mock<EmployerAccountOrchestrator>();

            _owinWrapper = new Mock<IAuthenticationService>();        
            new Mock<IMultiVariantTestingService>();
            var logger = new Mock<ILog>();
            new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _orchestrator.Setup(x => x.RenameEmployerAccount(It.IsAny<RenameEmployerAccountViewModel>(), It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<RenameEmployerAccountViewModel>
                {
                    Status = HttpStatusCode.OK,
                    Data = new RenameEmployerAccountViewModel()
                });

            _mockAccountCookieStorage = new Mock<ICookieStorageService<HashedAccountIdModel>>();

            _employerAccountController = new EmployerAccountController(_owinWrapper.Object,
                Mock.Of<EmployerAccountOrchestrator>(),
                Mock.Of<IMultiVariantTestingService>(),
                Mock.Of<ILog>(),
                Mock.Of<ICookieStorageService<FlashMessageViewModel>>(),
                Mock.Of<IMediator>(),
                Mock.Of<ICookieStorageService<ReturnUrlModel>>(),
                _mockAccountCookieStorage.Object)
            {
                ControllerContext = _controllerContext.Object,
                Url = new UrlHelper(new RequestContext(_httpContext.Object, new RouteData()), _routes)
            };
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("                        ")]
        public void Then_The_HashedAccountId_Is_Not_Stored(string hashedAccountId)
        {
            _employerAccountController.GatewayInform(hashedAccountId: hashedAccountId);

            _mockAccountCookieStorage
                .Verify(
                    m =>
                        m.Delete(It.IsAny<string>()),
                    Times.Never());

            _mockAccountCookieStorage
                .Verify(
                    m => m.Create(
                        It.IsAny<HashedAccountIdModel>(),
                        It.IsAny<string>(),
                        It.IsAny<int>()),
                    Times.Never
                );
        }

    }
}