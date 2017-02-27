using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.FeatureToggle;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.BaseControllerTests
{
    public class WhenOnActionExecuting : ControllerTestBase
    {
        private const string UserEmail = "user.one@unit.tests";

        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IUserWhiteList> _userWhiteList;
        private TestController _controller;

        [SetUp]
        public void Arrange()
        {
            base.Arrange();

            _owinWrapper = new Mock<IOwinWrapper>();
            _owinWrapper.Setup(x => x.GetClaimValue("email"))
                .Returns(UserEmail);

            _featureToggle = new Mock<IFeatureToggle>();
            _featureToggle.Setup(x => x.GetFeatures())
                .Returns(new FeatureToggleLookup { Data = new List<FeatureToggleItem>() });

            _userWhiteList = new Mock<IUserWhiteList>();


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
        public void ThenItShouldExecuteActionIfNotInFeatureToggles()
        {
            // Act
            var actual = Invoke(() => _controller.TestView());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ContentResult>(actual);
        }

        [TestCase("user\\.one@unit\\.tests")]
        [TestCase("USER\\.ONE@UNIT\\.TESTS")]
        [TestCase(".*@unit\\.tests")]
        public void ThenItShouldExecuteActionIfToggleEnabledAndUserInWhiteList(string whitelistPattern)
        {
            // Arrange
            _featureToggle.Setup(x => x.GetFeatures())
                .Returns(new FeatureToggleLookup
                {
                    Data = new List<FeatureToggleItem>
                    {
                        new FeatureToggleItem
                        {
                            Controller = "Test",
                            Action = "TestView",
                            WhiteList = new[] { whitelistPattern }
                        }
                    }
                });
            // Act
            var actual = Invoke(() => _controller.TestView());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ContentResult>(actual);
        }

        [Test]
        public void ThenItShouldReturnFeatureNotEnabledIfToggleEnabledWithNoWhiteList()
        {
            // Arrange
            _featureToggle.Setup(x => x.GetFeatures())
                .Returns(new FeatureToggleLookup
                {
                    Data = new List<FeatureToggleItem>
                    {
                        new FeatureToggleItem
                        {
                            Controller = "Test",
                            Action = "TestView",
                            WhiteList = new string[0]
                        }
                    }
                });

            // Act
            var actual = Invoke(() => _controller.TestView());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ViewResult>(actual);
            Assert.AreEqual("FeatureNotEnabled", ((ViewResult)actual).ViewName);
        }

        [Test]
        public void ThenItShouldReturnFeatureNotEnabledIfToggleEnabledButUserNotOnWhiteList()
        {
            // Arrange
            _featureToggle.Setup(x => x.GetFeatures())
                .Returns(new FeatureToggleLookup
                {
                    Data = new List<FeatureToggleItem>
                    {
                        new FeatureToggleItem
                        {
                            Controller = "Test",
                            Action = "TestView",
                            WhiteList = new[] { "different.user@somewhere.else" }
                        }
                    }
                });

            // Act
            var actual = Invoke(() => _controller.TestView());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ViewResult>(actual);
            Assert.AreEqual("FeatureNotEnabled", ((ViewResult)actual).ViewName);
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
        }
    }
}
