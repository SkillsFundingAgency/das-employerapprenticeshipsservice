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

        private Mock<IOperationAuthorisationHandler> _pipeline;
	private IAuthorizationContext _authorizationContext;
        private OperationAuthorisationService _operationAuthorisationService;

        [SetUp]
        public void Arrange()
        {
            _pipeline = new Mock<IOperationAuthorisationHandler>();
            _membershipContext = new Mock<IMembershipContext>();

            _pipeline.Setup(x => x.CanAccessAsync(It.IsAny<OperationContext>())).ReturnsAsync(true);

            _operationAuthorisationService = new OperationAuthorisationService(_pipeline.Object);
        }

        [Test]
        public void ThenShouldReturnThatFeatureIsEnabled()
        {
            //Act
            var result = _operationAuthorisationService.IsOperationAuthorised(ControllerName, ActionName, _membershipContext.Object);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ThenShouldReturnThatFeatureIsNotEnabled()
        {
            //Assign
            _pipeline.Setup(x => x.CanAccessAsync(It.IsAny<OperationContext>())).ReturnsAsync(false);

            //Act
            var result = _operationAuthorisationService.IsOperationAuthorised(ControllerName, ActionName, _membershipContext.Object);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ThenShouldRequestDetails()
        {
            //Act
            _operationAuthorisationService.IsOperationAuthorised(ControllerName, ActionName, _authorisationContext);

            //Assert
            _pipeline.Verify(x => x.CanAccessAsync(It.Is<OperationContext>(
                operationContext => operationContext.Controller.Equals(ControllerName) &&
                operationContext.Action.Equals(ActionName) &&
                operationContext.MembershipContext == _membershipContext.Object)), Times.Once);
        }
    }
}
