using System;
using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.Features;
using SFA.DAS.EAS.Infrastructure.Features;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Features
{
    [TestFixture]
    public class FeatureCacheFactoryTests
    {
        [Test]
        public void Create_WithNoFeatures_ShouldSucceed()
        {
            CheckWhenNoFeatures(cache => Assert.Pass("Should not get exception"));
        }

        [Test]
        public void Create_WithNoFeatures_ShouldNotGetNullResponse()
        {
            CheckWhenNoFeatures(Assert.IsNotNull);
        }

        [Test]
        public void Create_WithOneUnusedFeature_ShouldSucceed()
        {
            CheckWhenOneFeatureAdded((fixtures, cache) => Assert.Pass("Should not get exception"));
        }

        [Test]
        public void Create_WithOneUnusedFeature_ShouldNotGetNullResponse()
        {
            CheckWhenOneFeatureAdded((fixtures, cache) =>  Assert.IsNotNull(cache));
        }

        [Test]
        public void Create_WithOneUsedFeature_ShouldSucceed()
        {
            CheckWhenOneFeatureWithControllerAdded((fixtures, cache) => Assert.Pass("Should not get exception"));
        }

        [Test]
        public void Create_WithOneUsedFeature_ShouldNotGetNullResponse()
        {
            CheckWhenOneFeatureWithControllerAdded((fixtures, cache) => Assert.IsNotNull(cache));
        }

        [Test]
        public void Create_WithTwoUsedFeatures_ShouldSucceed()
        {
            CheckWhenTwoFeatureWithControllersAdded((fixtures, cache) => Assert.Pass("Should not get exception"));
        }

        [Test]
        public void Create_WithTwoUsedFeatures_ShouldNotGetNullResponse()
        {
            CheckWhenTwoFeatureWithControllersAdded((fixtures, cache) => Assert.IsNotNull(cache));
        }

        [Test]
        public void Create_WithOnedFeatureWithTwoEndpoints_ShouldSucceed()
        {
            CheckWhenOneFeatureWithTwoControllersAdded((fixtures, cache) => Assert.Pass("Should not get exception"));
        }

        [Test]
        public void Create_WithOnedFeatureWithTwoEndpoints_ShouldNotGetNullResponse()
        {
            CheckWhenOneFeatureWithTwoControllersAdded((fixtures, cache) => Assert.IsNotNull(cache));
        }
        
        private void CheckWhenNoFeatures(Action<IFeatureCache> check)
        {
            Check(null, (fixtures, cache) => check(cache));
        }

        private void CheckWhenOneFeatureAdded(Action<FeatureCacheFactoryTestFixtures, IFeatureCache> check)
        {
            Check(fixtures => fixtures.AddFeature(FeatureType.Test1), check);
        }

        private void CheckWhenOneFeatureWithControllerAdded(Action<FeatureCacheFactoryTestFixtures, IFeatureCache> check)
        {
            Check(fixtures => fixtures.AddFeature(FeatureType.Test1), check);
        }

        private void CheckWhenOneFeatureWithTwoControllersAdded(Action<FeatureCacheFactoryTestFixtures, IFeatureCache> check)
        {
            CheckWhenOneFeatureWithTwoControllersAdded(FeatureType.Test1, check);
        }

        private void CheckWhenOneFeatureWithTwoControllersAdded(FeatureType featureType, Action<FeatureCacheFactoryTestFixtures, IFeatureCache> check)
        {
            Check(fixtures => fixtures.AddFeature(featureType), check);
        }

        private void CheckWhenTwoFeatureWithControllersAdded(Action<FeatureCacheFactoryTestFixtures, IFeatureCache> check)
        {
            Check(fixtures => fixtures.AddFeature(FeatureType.Test1).AddFeature(FeatureType.Test2), check);
        }

        private void Check(Action<FeatureCacheFactoryTestFixtures> setup, Action<FeatureCacheFactoryTestFixtures, IFeatureCache> check)
        {
            // Arrange
            var fixtures = new FeatureCacheFactoryTestFixtures();

            setup?.Invoke(fixtures);

            var factory = fixtures.CreateFeatureToggleCacheFactory();

            // Act
            var cache = factory.Create(fixtures.Features);

            // Assert
            check?.Invoke(fixtures, cache);
        }
    }

    public class FeatureCacheFactoryTestFixtures
    {
        public List<string> ControllersAddedToFeatures { get; }
        public Feature[] Features => _explicitFeatures ?? _features.ToArray();

        private readonly List<Feature> _features;
        private Feature[] _explicitFeatures;

        public FeatureCacheFactoryTestFixtures()
        {
            ControllersAddedToFeatures = new List<string>();
            _features = new List<Feature>();
        }
        
        public FeatureCacheFactoryTestFixtures AddFeature(FeatureType featureType)
        {
            var feature = new Feature
            {
                FeatureType = featureType
            };

            _features.Add(feature);

            return this;
        }

        public FeatureCacheFactoryTestFixtures WithExplicitFeatures(Feature[] explicitFeatures)
        {
            _explicitFeatures = explicitFeatures;

            return this;
        }

        public FeatureCacheFactory CreateFeatureToggleCacheFactory()
        {
            return new FeatureCacheFactory();
        }
    }
}