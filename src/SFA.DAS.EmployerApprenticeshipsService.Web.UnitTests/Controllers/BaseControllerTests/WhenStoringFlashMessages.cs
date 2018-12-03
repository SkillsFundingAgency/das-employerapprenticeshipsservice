using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.BaseControllerTests
{
    public class WhenStoringFlashMessages : ControllerTestBase
    {
        private const string UserEmail = "user.one@unit.tests";
        private const string FlashMessageCookieName = "sfa-das-employerapprenticeshipsservice-flashmessage";
        
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IMultiVariantTestingService> _multiVariantTestingService;
        private TestController _controller;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

        [SetUp]
        public void Arrange()
        { 
            base.Arrange();
            _owinWrapper = new Mock<IAuthenticationService>();
            _owinWrapper.Setup(x => x.GetClaimValue("email"))
                .Returns(UserEmail);

            _multiVariantTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            var routes = new RouteData();
            routes.Values["action"] = "TestView";
            routes.Values["controller"] = "Test";
            _controllerContext.Setup(x => x.RouteData).Returns(routes);

            _controller = new TestController(_owinWrapper.Object, 
                _multiVariantTestingService.Object, _flashMessage.Object)
            {
                ControllerContext = _controllerContext.Object
            };
        }

        [Test]
        public void ThenCookieIsClearedAndTheModelIsStoredInTheCookie()
        {
            //Act
            _controller.TestView();

            //Assert
            _flashMessage.Verify(x=>x.Delete(FlashMessageCookieName), Times.Once);
            _flashMessage.Verify(x=>x.Create(It.IsAny<FlashMessageViewModel>(),FlashMessageCookieName,1), Times.Once);
        }

        [Test]
        public void ThenItIsRetrievedFromTheCookieThenDeleted()
        {
            //Arrange
            _flashMessage.Setup(x => x.Get(FlashMessageCookieName)).Returns(new FlashMessageViewModel {Headline = "test"});

            //Act
            var actual = _controller.TestModel();

            //Assert
            _flashMessage.Verify(x => x.Delete(FlashMessageCookieName), Times.Once);
            _flashMessage.Verify(x => x.Get(FlashMessageCookieName), Times.Once);
            Assert.IsAssignableFrom<FlashMessageViewModel>(actual);
            Assert.AreEqual("test",actual.Headline);

        }

        internal class TestController : BaseController
        {
            public TestController(IAuthenticationService owinWrapper, 
                IMultiVariantTestingService multiVariantTestingService, ICookieStorageService<FlashMessageViewModel> flashMessage)
                : base(owinWrapper, multiVariantTestingService, flashMessage)
            {

            }

            public ActionResult TestView()
            {
                AddFlashMessageToCookie(new FlashMessageViewModel());
                return new ContentResult();
            }

            public FlashMessageViewModel TestModel()
            {
                return GetFlashMessageViewModelFromCookie();
            }

            public ActionResult TestFeatureView()
            {
                return View("TestView", new OrchestratorResponse<string>());
            }
            
        }
    }
}
