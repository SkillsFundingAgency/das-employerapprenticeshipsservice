using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Charts;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Services.Features;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.Features.FeatureWhiteListingServiceTests
{
    [TestFixture]
    public class WhenICallIsFeaturedEnabledForContext
    {
        [Test]
        public void Constructor()
        {
            var whiteListingService = new FeatureWhiteListingServiceTestFixtures().CreateFeatureWhiteListingService();

            Assert.Pass("Shouldn't get an exception");
        }

        [TestCase(true, "fredflintstone@bedrock.com", "fredflintstone@bedrock.com")]
        [TestCase(false, "fredflintstone@bedrock.com", "barneyrubble@bedrock.com")]
        [TestCase(false, "fredflintstone@bedrock.com")]
        [TestCase(true, "fredflintstone@bedrock.com", FeatureWhiteListingServiceTestFixtures.Nullstring)]
        public async Task TestEmailAssessment(bool expectedIsEnabled, string userEmail, params string[] whitelist)
        {
            // Arrange
            var fixtures = new FeatureWhiteListingServiceTestFixtures()
                                .WithUserEmail(userEmail)
                                .WithWhiteListed(whitelist);

            var service = fixtures.CreateFeatureWhiteListingService();
            var operationContext = fixtures.CreateOperationContext();

            // Act
            var actualIsEnabled =
                await service.IsFeatureEnabledForContextAsync(operationContext);

            // Assert
            Assert.AreEqual(expectedIsEnabled, actualIsEnabled);
        }
    }

    internal class FeatureWhiteListingServiceTestFixtures
    {
        public FeatureWhiteListingServiceTestFixtures()
        {
            FeatureServiceMock = new Mock<IFeatureService>();    
            MembershipContextMock = new Mock<IMembershipContext>();
        }

        public Mock<IFeatureService> FeatureServiceMock { get; }
        public IFeatureService FeatureService => FeatureServiceMock.Object;

        public Mock<IMembershipContext> MembershipContextMock { get; }
        public IMembershipContext MembershipContext => MembershipContextMock.Object;

        public FeatureWhiteListingService CreateFeatureWhiteListingService()
        {
            return new FeatureWhiteListingService(FeatureService);
        }

        public OperationContext CreateOperationContext()
        {
            var ca = new ControllerAction("home.index");

            return new OperationContext
            {
                Action = ca.Action,
                Controller = ca.Controller,
                MembershipContext = MembershipContext
            }; 
        }

        public FeatureWhiteListingServiceTestFixtures WithUserEmail(string userEmail)
        {
            MembershipContextMock.Setup(mc => mc.UserEmail).Returns(userEmail);

            return this;
        }

        public const string Nullstring = "null";

        public FeatureWhiteListingServiceTestFixtures WithWhiteListed(string[] whitelist)
        {
            // treat a single element array with only special null string as a null array 
            if (whitelist.Length == 1 && whitelist[0] == Nullstring)
            {
                whitelist = null;
            }

            FeatureServiceMock
                .Setup(fsm => fsm.GetWhitelistForOperationAsync(It.IsAny<OperationContext>()))
                .ReturnsAsync(whitelist);

            return this;
        }
    }
}
