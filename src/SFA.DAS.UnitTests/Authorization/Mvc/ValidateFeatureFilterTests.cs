using System.Net;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.Authorization.Mvc;

namespace SFA.DAS.UnitTests.Authorization.Mvc
{
    [TestFixture]
    public class ValidateFeatureFilterTests
    {
        private ValidateFeatureFilter _filter;
        private ActionExecutingContext _filterContext;
        private readonly Controller _controller = new ControllerStub();
        private Mock<IAuthorizationService> _authorizationService;
        
        [SetUp]
        public void Arrange()
        {
            _filterContext = new ActionExecutingContext
            {
                ActionDescriptor = new ReflectedActionDescriptor(
                    _controller.GetType().GetMethod(nameof(ControllerStub.Index)),
                    nameof(ControllerStub.Index),
                    new ReflectedAsyncControllerDescriptor(_controller.GetType()))
            };

            _authorizationService = new Mock<IAuthorizationService>();

            _filter = new ValidateFeatureFilter(() => _authorizationService.Object);
        }

        [Test]
        public void WhenIViewAPageThenIShouldBeShownThePageIfTheFeatureIsEnabledAndIAmLoggedIn()
        {
	        _authorizationService.Setup(a => a.IsAuthorized(FeatureType.Test1)).Returns(true);
            _filter.OnActionExecuting(_filterContext);

            Assert.That(_filterContext.Result, Is.Null);
        }

		[Test]
        public void WhenIViewAPageThenIShouldNotBeShownThePageIfTheFeatureIsDisabled()
        {
            _authorizationService.Setup(f => f.IsAuthorized(FeatureType.Test1)).Returns(false);

            _filter.OnActionExecuting(_filterContext);

            var result = _filterContext.Result as HttpStatusCodeResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        private class ControllerStub : Controller
        {
            [Feature(FeatureType.Test1)]
            public ActionResult Index()
            {
                return View();
            }
        }
    }
}