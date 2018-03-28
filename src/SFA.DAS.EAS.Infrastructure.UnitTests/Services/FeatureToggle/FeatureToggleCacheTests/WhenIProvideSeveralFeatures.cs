using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.FeatureToggle.FeatureToggleCacheTests
{
    [TestFixture]
    public class WhenIProvideSeveralFeatures
    {
        [Test]
        public void ThenFeatureShouldBeSubjectToAToggle()
        {
            // arrange
            var fixtures = new ToggleFeatureTestFixtures()
                .WithFeature((Feature) 0, "controller1.index")
                .WithFeature((Feature) 1, "controller2.index");

            var ftc = fixtures.CreateFixtureCache();

            // act
            var isControllerSubjectToToggle = ftc.IsControllerSubjectToFeatureToggle("controller1");

            // assert
            Assert.IsTrue(isControllerSubjectToToggle);
        }

        [Test]
        public void ThenActionShouldBeSubjectToAToggle()
        {
            // arrange
            var fixtures = new ToggleFeatureTestFixtures()
                .WithFeature((Feature)0, "controller1.index")
                .WithFeature((Feature)1, "controller2.index");

            var ftc = fixtures.CreateFixtureCache();

            // act
            var isControllerSubjectToToggle = ftc.TryGetControllerActionSubjectToToggle("controller1", "index", out _);

            // assert
            Assert.IsTrue(isControllerSubjectToToggle);
        }
    }
}