using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Pipeline;
using SFA.DAS.EAS.Infrastructure.Services.FeatureToggle;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.FeatureToggleServiceTests
{
    [TestFixture]
    public class WhenICheckIfFeatureIsEnabled
    {
        private const string ControllerName = "Test_Controller";
        private const string ActionName = "Test_Action";

        private Mock<ILog> _logger;
        private Mock<IPipeline<FeatureToggleRequest, bool>> _pipeline;
        private Mock<IMembershipContext> _membershipContext;
        private FeatureToggleService _featureToggleService;

        [SetUp]
        public void Arrange()
        {
            _pipeline = new Mock<IPipeline<FeatureToggleRequest, bool>>();
            _membershipContext = new Mock<IMembershipContext>();
            _logger = new Mock<ILog>();

            _pipeline.Setup(x => x.ProcessAsync(It.IsAny<FeatureToggleRequest>())).ReturnsAsync(true);

            _featureToggleService = new FeatureToggleService(_pipeline.Object, _logger.Object);
        }

        [Test]
        public void ThenShouldReturnThatFeatureIsEnabled()
        {
            //Act
            var result = _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, _membershipContext.Object);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ThenShouldReturnThatFeatureIsNotEnabled()
        {
            //Assign
            _pipeline.Setup(x => x.ProcessAsync(It.IsAny<FeatureToggleRequest>())).ReturnsAsync(false);

            //Act
            var result = _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, _membershipContext.Object);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ThenShouldRequestDetails()
        {
            //Act
            _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, _membershipContext.Object);

            //Assert
            _pipeline.Verify(x => x.ProcessAsync(It.Is<FeatureToggleRequest>(
                request => request.Controller.Equals(ControllerName) &&
                request.Action.Equals(ActionName) &&
                request.MembershipContext == _membershipContext.Object)), Times.Once);
        }
    }
}
