using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Pipeline.Features.Handlers;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Services.Features;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Pipeline.Features.Handlers
{
    [TestFixture]
    public class AgreementFeatureAuthorisationHandlerTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public async Task CanAccessAsync_WhenUserHasSignedRelevantAgreement_ShouldReturnSameCanAccess(bool shouldFeatureBeEnabled)
        {
            //Arrange
            var featureAgreementService = new Mock<IFeatureAgreementService>();

            featureAgreementService
                .Setup(fts => fts.IsFeatureEnabled(It.IsAny<OperationContext>()))
                .ReturnsAsync(shouldFeatureBeEnabled);

            var handler = new AgreementFeatureAuthorisationHandler(featureAgreementService.Object);

            // Act
            var actualCanAccess = await handler.CanAccessAsync(new OperationContext());

            // Assert
            Assert.AreEqual(shouldFeatureBeEnabled, actualCanAccess);
        }
    }
}
