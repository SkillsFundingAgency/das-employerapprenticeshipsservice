using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.FeatureToggle.FeatureToggleCacheTests
{
    [TestFixture]
    public class WhenIProvideNoFeatures
    {
        [Test]
        public void ThenNoFeaturesShouldBeSubjectToAToggle()
        {
            // arrange
            var ftc = new Infrastructure.Services.FeatureToggle.FeatureToggleCache(FeatureToggleCollectionBuilder.Create());

            // act
            var isControllerSubjectToToggle = ftc.IsControllerSubjectToFeatureToggle("foo");

            // assert
            Assert.IsFalse(isControllerSubjectToToggle);
        }

        [Test]
        public void ThenNoActionShouldBeSubjectToAToggle()
        {
            // arrange
            var ftc = new Infrastructure.Services.FeatureToggle.FeatureToggleCache(FeatureToggleCollectionBuilder.Create());

            // act
            var isActionSubjectToToggle = ftc.TryGetControllerActionSubjectToToggle("foo", "action", out _);

            // assert
            Assert.IsFalse(isActionSubjectToToggle);
        }
    }

    internal static class StringExtensions
    {
        public static string InvertCase(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return s;
            }

            var newString = new char[s.Length];

            for (int i = 0; i < newString.Length; i++)
            {
                var c = s[i];
                if (char.IsLetter(c))
                {
                    c = char.IsLower(c) ? char.ToUpper(c) : char.ToLower(c);
                }

                newString[i] = c;
            }

            return new string(newString);
        }
    }

    internal static class FeatureToggleCollectionBuilder
    {
        public static FeatureToggleCollection Create()
        {
            return new FeatureToggleCollection();
        }

        public static FeatureToggleCollection WithFeature(this FeatureToggleCollection featureToggleCollection,
            Domain.Models.FeatureToggles.FeatureToggle featureToggle)
        {
            featureToggleCollection.Features.Add(featureToggle);
            return featureToggleCollection;
        }
    }

    internal static class FeatureToggleBuilder
    { 
        public static Domain.Models.FeatureToggles.FeatureToggle Create(string name)
        {
            var newFeature = new Domain.Models.FeatureToggles.FeatureToggle { Name = name };
            return newFeature;
        }

        public static Domain.Models.FeatureToggles.FeatureToggle WithControllerAction(this Domain.Models.FeatureToggles.FeatureToggle featureToggle, string controller, string action)
        {
            featureToggle.Actions.Add(new ControllerAction {Controller = controller, Action = action});
            return featureToggle;
        }


        public static Domain.Models.FeatureToggles.FeatureToggle WithWhitelistedEmail(this Domain.Models.FeatureToggles.FeatureToggle featureToggle, string whitelistedEmail)
        {
            featureToggle.Whitelist.Emails.Add(whitelistedEmail);

            return featureToggle;
        }
    }
}
