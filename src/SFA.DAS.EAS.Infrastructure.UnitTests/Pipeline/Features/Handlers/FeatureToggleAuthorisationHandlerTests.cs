using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Pipeline.Features.Handlers;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Infrastructure.Services.Features;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Pipeline.Features.Handlers
{
    [TestFixture]
    public class FeatureToggleAuthorisationHandlerTests
    {
        [Test]
        public void Constructor()
        {
            var whiteListingService = new FeatureWhitelistAuthorisationHandler();

            Assert.Pass("Shouldn't get an exception");
        }

        [TestCase(true, "fredflintstone@bedrock.com", "fredflintstone@bedrock.com")]
        [TestCase(false, "fredflintstone@bedrock.com", "barneyrubble@bedrock.com")]
        [TestCase(false, "fredflintstone@bedrock.com")]
        [TestCase(true, "fredflintstone@bedrock.com", FeatureToggleAuthorisationHandlerTestsFixtures.Nullstring)]
        public async Task TestEmailAssessment(bool expectedIsEnabled, string userEmail, params string[] whitelist)
        {
            // Arrange
            var fixtures = new FeatureToggleAuthorisationHandlerTestsFixtures()
                .WithUserEmail(userEmail)
                .WithWhitelist(whitelist);

            var operationContext = fixtures.CreateOperationContext();
            var handler = new FeatureWhitelistAuthorisationHandler();

            // Act
            var actualIsEnabled =
                await handler.IsFeatureEnabledForContextAsync(operationContext);

            // Assert
            Assert.AreEqual(expectedIsEnabled, actualIsEnabled);
        }

    }

    internal class FeatureToggleAuthorisationHandlerTestsFixtures
    {
        public FeatureToggleAuthorisationHandlerTestsFixtures()
        {
            UserContextMock = new Mock<IUserContext>();
        }

        public Mock<IUserContext> UserContextMock { get; }
        public IUserContext UserContext => UserContextMock.Object;

        public IAuthorizationContext CreateOperationContext()
        {
            var ca = new ControllerAction("home.index");

            return new AuthorizationContext 
            {
                CurrentFeature = new Feature { Enabled = true, Whitelist = Whitelist},
                UserContext = UserContext
            };
        }

        public FeatureToggleAuthorisationHandlerTestsFixtures WithUserEmail(string userEmail)
        {
            UserContextMock.Setup(uc => uc.Email).Returns(userEmail);

            return this;
        }

        public const string Nullstring = "null";

        public string[] Whitelist { get; set; }

        public FeatureToggleAuthorisationHandlerTestsFixtures WithWhitelist(string[] whitelist)
        {
            // treat a single element array with only special null string as a null array 
            if (whitelist.Length == 1 && whitelist[0] == Nullstring)
            {
                whitelist = null;
            }

            Whitelist = whitelist;

            return this;
        }
    }
}
