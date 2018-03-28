using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Services.FeatureToggle;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.FeatureToggle.FeatureToggleCacheTests
{
    [TestFixture]
    public class WhenIProvideSeveralFeaturesWithSharedControllers
    {
        private const string SharedControllerName = "SharedController";
        private const string SharedActionName = "SharedActionName";

        [Test]
        public void ThenFeatureShouldBeSubjectToAToggle()
        {
            // arrange
            var fixtures = new ToggleFeatureTestFixtures().BuildToggledFeatures(3, $"{SharedControllerName}.{SharedActionName}");

            var ftc = fixtures.CreateFixtureCache();

            // act
            var isActionSubjectToToggle = ftc.IsControllerSubjectToFeatureToggle(SharedControllerName);

            // assert
            Assert.IsTrue(isActionSubjectToToggle);
        }

        [Test]
        public void ThenActionShouldBeSubjectToAToggle()
        {
            // arrange
            var fixtures = new ToggleFeatureTestFixtures().BuildToggledFeatures(3, $"{SharedControllerName}.{SharedActionName}");
            var ftc = fixtures.CreateFixtureCache();

            // act
            var isActionSubjectToToggle = ftc.TryGetControllerActionSubjectToToggle(SharedControllerName, SharedActionName, out _);

            // assert
            Assert.IsTrue(isActionSubjectToToggle);
        }

        [Test]
        public void ThenWhitelistsShouldBeAppendedTogether()
        {
            // arrange
            var fixtures = new ToggleFeatureTestFixtures().BuildToggledFeatures(3, $"{SharedControllerName}.{SharedActionName}");
            var ftc = fixtures.CreateFixtureCache();

            // act
            var isActionSubjectToToggle = ftc.TryGetControllerActionSubjectToToggle(SharedControllerName, SharedActionName, out var cachedItem);

            // assert
            const int expectedNumberOfWhitelists = 3;
            Assert.AreEqual(expectedNumberOfWhitelists, cachedItem.WhiteLists.Count);
            Assert.AreEqual("email-0", cachedItem.WhiteLists[0].Emails[0]);
            Assert.AreEqual("email-1", cachedItem.WhiteLists[1].Emails[0]);
            Assert.AreEqual("email-2", cachedItem.WhiteLists[2].Emails[0]);
        }
    }
}