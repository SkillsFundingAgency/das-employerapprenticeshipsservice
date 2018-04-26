using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.Features;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Infrastructure.Features;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Features
{
    [TestFixture]
    public class FeatureWhitelistAuthorisationHandlerTests
    {
        [TestCase(AuthorizationResult.Ok, "fredflintstone@bedrock.com", "fredflintstone@bedrock.com")]
        [TestCase(AuthorizationResult.FeatureUserNotWhitelisted, "fredflintstone@bedrock.com", "barneyrubble@bedrock.com")]
        [TestCase(AuthorizationResult.FeatureUserNotWhitelisted, "fredflintstone@bedrock.com")]
        [TestCase(AuthorizationResult.Ok, "fredflintstone@bedrock.com", FeatureToggleAuthorisationHandlerTestsFixtures.Nullstring)]
        public async Task TestEmailAssessment(AuthorizationResult expectedIsEnabled, string userEmail, params string[] whitelist)
        {
            // Arrange
            var fixtures = new FeatureToggleAuthorisationHandlerTestsFixtures()
                .WithUserEmail(userEmail)
                .WithWhitelist(whitelist);

            var authorizationContext = fixtures.CreateAuthorizationContext();
            var feature = fixtures.CreateFeature();
            var handler = new FeatureWhitelistAuthorizationHandler();

            // Act
            var isEnabled = await handler.CanAccessAsync(authorizationContext, feature);

            // Assert
            Assert.AreEqual(expectedIsEnabled, isEnabled);
        }

    }

    public class FeatureToggleAuthorisationHandlerTestsFixtures
    {
        public Mock<IUserContext> UserContextMock { get; }
        public IUserContext UserContext => UserContextMock.Object;
        public string[] Whitelist { get; set; }

        public const string Nullstring = "null";

        public FeatureToggleAuthorisationHandlerTestsFixtures()
        {
            UserContextMock = new Mock<IUserContext>();
        }

        public IAuthorizationContext CreateAuthorizationContext()
        {
            return new AuthorizationContext 
            {
                UserContext = UserContext
            };
        }

        public Feature CreateFeature()
        {
            return new Feature { Enabled = true, Whitelist = Whitelist };
        }

        public FeatureToggleAuthorisationHandlerTestsFixtures WithUserEmail(string userEmail)
        {
            UserContextMock.Setup(u => u.Email).Returns(userEmail);

            return this;
        }

        public FeatureToggleAuthorisationHandlerTestsFixtures WithWhitelist(string[] whitelist)
        {
            // Treat a single element array with only special null string as a null array 
            if (whitelist.Length == 1 && whitelist[0] == Nullstring)
            {
                whitelist = null;
            }

            Whitelist = whitelist;

            return this;
        }
    }
}