using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.Features.FeatureToggleCacheTests
{
    [TestFixture]
    public class WhenIProvideSeveralFeatures
    {
        [Test]
        public void ThenFeatureShouldBeSubjectToAToggle()
        {
            // arrange
            var fixtures = new ToggleFeatureTestFixtures()
                .WithFeature((FeatureType) 0, "controller1.index")
                .WithFeature((FeatureType) 1, "controller2.index");

            var ftc = fixtures.CreateFixtureCache();

            // act
            var isControllerSubjectToToggle = ftc.IsControllerSubjectToFeature("controller1");

            // assert
            Assert.IsTrue(isControllerSubjectToToggle);
        }

        [Test]
        public void ThenActionShouldBeSubjectToAToggle()
        {
            // arrange
            var fixtures = new ToggleFeatureTestFixtures()
                .WithFeature((FeatureType)0, "controller1.index")
                .WithFeature((FeatureType)1, "controller2.index");

            var ftc = fixtures.CreateFixtureCache();

            // act
            var isControllerSubjectToToggle = ftc.TryGetControllerActionSubjectToFeature("controller1", "index", out _);

            // assert
            Assert.IsTrue(isControllerSubjectToToggle);
        }
    }
}