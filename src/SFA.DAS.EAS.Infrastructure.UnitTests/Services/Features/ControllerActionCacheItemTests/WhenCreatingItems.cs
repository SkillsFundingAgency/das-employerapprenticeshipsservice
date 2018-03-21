using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Services.Features;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.FeatureToggle.ControllerActionCacheItemTests
{
    [TestFixture]
    public class WhenCreatingItems
    {
        [TestCase("controller1.index", "controller1.index", true)]
        [TestCase("CoNtRoLlEr1.index", "controller1.index", true)]
        [TestCase("controller1.InDeX", "controller1.index", true)]
        [TestCase("CoNtRoLlEr1.InDeX", "controller1.index", true)]
        [TestCase("controller1.index", "controller2.index", false)]
        [TestCase("controller1.index1", "controller1.index2", false)]
        [TestCase("controller1.index1", "controller2.index2", false)]
        public void HashShouldBeAsExpected(string controllerAction1, string controllerAction2, bool expectedMatch)
        {
            // Action
            var cacheItem1 = Create(controllerAction1);
            var cacheItem2 = Create(controllerAction2);

            // Assert
            var actualHashMatch = cacheItem1.GetHashCode() == cacheItem2.GetHashCode();
            var actualEqualityMatch = cacheItem1.Equals(cacheItem2);

            var actualMatch = actualHashMatch && actualEqualityMatch;

            Assert.AreEqual(expectedMatch, actualMatch);
        }

        private ControllerActionCacheItem Create(string controllerActionName)
        {
            var controllerAction = new ControllerAction(controllerActionName);

            return new ControllerActionCacheItem(controllerAction, new Feature());
        }
    }
}
