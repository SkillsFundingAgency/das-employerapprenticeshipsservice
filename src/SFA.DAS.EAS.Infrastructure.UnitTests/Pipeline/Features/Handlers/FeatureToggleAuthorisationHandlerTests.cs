﻿using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Pipeline.Features.Handlers;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Pipeline.Features.Handlers
{
    [TestFixture]
    public class FeatureToggleAuthorisationHandlerTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public async Task CanAccessAsync_WhenOperationHasSpecifiedEnabledState__ShouldReturnSameCanAccess(bool shouldFeatureBeEnabled)
        {
            //Arrange
            var featureToggleServiceMock = new Mock<IFeatureToggleService>();

            featureToggleServiceMock
                .Setup(fts => fts.IsFeatureEnabled(It.IsAny<OperationContext>()))
                .ReturnsAsync(shouldFeatureBeEnabled);

            var handler = new FeatureToggleAuthorisationHandler(featureToggleServiceMock.Object);

            // Act
            var actualCanAccess = await handler.CanAccessAsync(new OperationContext());

            // Assert
            Assert.AreEqual(shouldFeatureBeEnabled, actualCanAccess);
        }
    }
}
