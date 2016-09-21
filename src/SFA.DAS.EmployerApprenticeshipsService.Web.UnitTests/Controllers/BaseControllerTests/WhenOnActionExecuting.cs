using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.FeatureToggle;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Controllers;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Controllers.BaseControllerTests
{
    public class WhenOnActionExecuting : ControllerTestBase
    {
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IUserWhiteList> _userWhiteList;
        private TestController _controller;

        [SetUp]
        public void Arrange()
        {
            base.Arrange();

            _featureToggle = new Mock<IFeatureToggle>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _userWhiteList = new Mock<IUserWhiteList>();
            _featureToggle.Setup(x => x.GetFeatures()).Returns(new FeatureToggleLookup {Data = new List<FeatureToggleItem>()});

            var routes = new RouteData();
            routes.Values["action"] = "TestView";
            routes.Values["controller"] = "Test";
            _controllerContext.Setup(x => x.RouteData).Returns(routes);
            
            _controller = new TestController(_featureToggle.Object, _owinWrapper.Object, _userWhiteList.Object)
            {
                ControllerContext = _controllerContext.Object
            };
        }

        [Test]
        public void ThenTheFeatureIsCheckedToSeeIfEnabled()
        {
            //Act
            Invoke(() => _controller.TestView());
            
            //Assert
            _featureToggle.Verify(x=>x.GetFeatures(),Times.Once);
        }

        [TestCase("Test","*",false)]
        [TestCase("Test","TestView",false)]
        [TestCase("TEST","TESTVIEW",false)]
        [TestCase("Test","TestV",true)]
        [TestCase("T","TestView",true)]
        public void ThenTheFeatureNotEnabledViewIsReturnedIfItMatches(string controllerName,string actionName,bool enabled)
        {
            //Arrange
            _featureToggle.Setup(x => x.GetFeatures()).Returns(new FeatureToggleLookup { Data = new List<FeatureToggleItem>
            {
                new FeatureToggleItem
                {
                    Controller = controllerName,
                    Action = actionName
                }
            } });

            //Act
            var result = Invoke(() => _controller.TestView());

            //Assert
            if (enabled)
            {
                Assert.IsAssignableFrom<ContentResult>(result);
            }
            else
            {
                Assert.IsAssignableFrom<ViewResult>(result);
                var viewResult = result as ViewResult;
                Assert.IsNotNull(viewResult);
                Assert.AreEqual("FeatureNotEnabled",viewResult.ViewName);
            }
        }
       
        public void ThenShouldNotDirectToUserNotAllowedPageIfUserIsOnWhiteList()
        {
            //Assign
            _userWhiteList.Setup(x => x.IsEmailOnWhiteList(It.IsAny<string>())).Returns(true);
            _owinWrapper.Setup(x => x.GetClaimValue("email")).Returns("test@test.com");

            //Act
            var result = Invoke(() => _controller.SecureTestView());

            //Assert
            Assert.IsAssignableFrom<ContentResult>(result);
        }

        public void ThenShouldDirectToUserNotAllowedPageIfUserIsNotOnWhiteList()
        {
            //Assign
            _userWhiteList.Setup(x => x.IsEmailOnWhiteList(It.IsAny<string>())).Returns(false);
            _owinWrapper.Setup(x => x.GetClaimValue("email")).Returns("test@test.com");

            //Act
            var result = Invoke(() => _controller.SecureTestView());

            //Assert
            Assert.IsAssignableFrom<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("UserNotAllowed", viewResult.ViewName);
        }
        
        public void ThenShouldNoDirectToUserNotAllowedPageIfPublicWebPage()
        {
            //Assign
            _userWhiteList.Setup(x => x.IsEmailOnWhiteList(It.IsAny<string>())).Returns(false);
            _owinWrapper.Setup(x => x.GetClaimValue("email")).Returns("test@test.com");

            //Act
            var result = Invoke(() => _controller.TestView());

            //Assert
            Assert.IsAssignableFrom<ContentResult>(result);
        }

        internal class TestController : BaseController
        {
            public TestController(IFeatureToggle featureToggle, IOwinWrapper owinWrapper, IUserWhiteList userWhiteList) 
                : base(owinWrapper, featureToggle, userWhiteList)
            {

            }

            public ActionResult TestView()
            {
                return new ContentResult();
            }

            [Authorize]
            public ActionResult SecureTestView()
            {
                return new ContentResult();
            }
        }
    }
}
