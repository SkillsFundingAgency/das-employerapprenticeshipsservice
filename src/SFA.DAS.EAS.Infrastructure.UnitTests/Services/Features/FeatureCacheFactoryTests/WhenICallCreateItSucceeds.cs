using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Services.Features;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.Features.FeatureCacheFactoryTests
{
    [TestFixture]
    public class WhenICallCreateItSucceeds
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
        public void Create_WithOneUsedFeature_ShouldReportControllerAsPartOfFeature()
        {
            CheckWhenOneFeatureWithControllerAdded((fixtures, cache) => Assert.IsTrue(cache.IsControllerSubjectToFeature(fixtures.ControllersAddedToFeatures[0])));
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
        public void Create_WithTwoUsedFeatures_ShouldReportControllersAsPartOfFeature()
        {
            CheckWhenTwoFeatureWithControllersAdded((fixtures, cache) =>
            {
                Assert.IsTrue(cache.IsControllerSubjectToFeature(fixtures.ControllersAddedToFeatures[0]));
                Assert.IsTrue(cache.IsControllerSubjectToFeature(fixtures.ControllersAddedToFeatures[1]));
            });
        }

        [Test]
        public void Create_WithTwoUsedFeatures_ShouldCacheCorrectDetails()
        {
            CheckWhenTwoFeatureWithControllersAdded((fixtures, cache) =>
            {
                foreach (var controllerAction in fixtures.QualifiedEndpointsAdded)
                {
                    var operationContext = new OperationContext
                    {
                        Action = controllerAction.Action,
                        Controller = controllerAction.Controller
                    };

                    var wasFound = cache.TryGetControllerActionSubjectToFeature(operationContext, out var cacheitem);

                    Assert.IsTrue(wasFound, $"{controllerAction.QualifiedName} was not reported as a feature endpoint");
                    Assert.IsNotNull(cacheitem, $"{controllerAction.QualifiedName} was reported as a feature endpoint but was not found in the cache");
                    Assert.AreEqual(controllerAction.Controller, cacheitem.Controller,  $"{controllerAction.QualifiedName} found in the cache had the wrong controller name");
                    Assert.AreEqual(controllerAction.Action, cacheitem.Action, $"{controllerAction.QualifiedName} found in the cache had the wrong action name");
                }
            });
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

        [Test]
        public void Create_WithOnedFeatureWithTwoEndpoints_ShouldReportControllersAsPartOfFeature()
        {
            CheckWhenOneFeatureWithTwoControllersAdded((fixtures, cache) =>
            {
                Assert.IsTrue(cache.IsControllerSubjectToFeature(fixtures.ControllersAddedToFeatures[0]));
            });
        }

        [Test]
        public void Create_WithOneFeatureWithTwoEndpoints_ShouldReportEndpointsAsPartOfSpecificFeature()
        {
            const FeatureType featureToUseForTest = FeatureType.Test1;

            CheckWhenOneFeatureWithTwoControllersAdded(featureToUseForTest, (fixtures, cache) =>
            {
                foreach (var controllerAction in fixtures.QualifiedEndpointsAdded)
                {
                    var operationContext = new OperationContext
                    {
                        Action = controllerAction.Action,
                        Controller = controllerAction.Controller
                    };

                    var wasFound = cache.TryGetControllerActionSubjectToFeature(operationContext, out var cacheitem);

                    Assert.IsTrue(wasFound, $"Did not find {controllerAction.QualifiedName}");
                    Assert.IsNotNull(cacheitem.Feature, $"The feature attached to {controllerAction.QualifiedName} is null");
                    Assert.AreEqual(featureToUseForTest, cacheitem.FeatureType, $"The feature associated with {controllerAction.QualifiedName} is not the expected one");
                }
            });
        }

        #region TestHelpers
        private void CheckWhenNoFeatures(Action<IFeatureCache> check)
        {
            Check(null, (fixtures, cache) => check(cache));
        }

        private void CheckWhenNullFeatures(Action<IFeatureCache> check)
        {
            Check(fixtures => fixtures.WithExplicitFeatures(null), (fixtures, cache) => check(cache));
        }

        private void CheckWhenOneFeatureAdded(Action<FeatureCacheFactoryTestFixtures, IFeatureCache> check)
        {
            Check(fixtures => fixtures.AddFeature(FeatureType.Test1), check);
        }

        private void CheckWhenOneFeatureWithControllerAdded(Action<FeatureCacheFactoryTestFixtures, IFeatureCache> check)
        {
            Check(fixtures => fixtures
                                .AddFeature(FeatureType.Test1)
                                .WithEndpoints(FeatureType.Test1, "controller1.index"), check);
        }

        private void CheckWhenOneFeatureWithTwoControllersAdded(Action<FeatureCacheFactoryTestFixtures, IFeatureCache> check)
        {
            CheckWhenOneFeatureWithTwoControllersAdded(FeatureType.Test1, check);
        }

        private void CheckWhenOneFeatureWithTwoControllersAdded(FeatureType featureType, Action<FeatureCacheFactoryTestFixtures, IFeatureCache> check)
        {
            Check(fixtures => fixtures
                .AddFeature(featureType)
                .WithEndpoints(featureType, "controller1.index", "controller2.index"), check);
        }

        private void CheckWhenTwoFeatureWithControllersAdded(Action<FeatureCacheFactoryTestFixtures, IFeatureCache> check)
        {
            Check(
                setup: fixtures => fixtures
                        .AddFeature(FeatureType.Test1)
                        .WithEndpoints(FeatureType.Test1, "controller1.index")
                        .AddFeature(FeatureType.Test2)
                        .WithEndpoints(FeatureType.Test2, "controller2.index"),
                check:check);
        }

        private void Check(
            Action<FeatureCacheFactoryTestFixtures> setup, 
            Action<FeatureCacheFactoryTestFixtures, IFeatureCache> check)
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
        #endregion
    }

    public class FeatureCacheFactoryTestFixtures
    {
        private readonly List<Feature> _features;
        private Feature[] _explicitFeatures;

        public FeatureCacheFactoryTestFixtures() {
            ControllerMetaDataServiceMock = new Mock<IControllerMetaDataService>();    
            _features = new List<Feature>();
            ControllersAddedToFeatures = new List<string>();
            QualifiedEndpointsAdded = new List<ControllerAction>();
        } 

        public Mock<IControllerMetaDataService> ControllerMetaDataServiceMock { get; }
        public IControllerMetaDataService ControllerMetaDataService => ControllerMetaDataServiceMock.Object;

        public Feature[] Features => _explicitFeatures ?? _features.ToArray();

        public FeatureCacheFactoryTestFixtures AddFeature(FeatureType featureType)
        {
            var feature = new Feature
            {
                FeatureType = featureType
            };

            _features.Add(feature);

            return this;
        }

        public FeatureCacheFactoryTestFixtures WithEndpoints(FeatureType featureType, params string[] endpointsInFeature)
        {
            var controllerActions = endpointsInFeature.Select(s => new ControllerAction(s)).ToArray();

            ControllerMetaDataServiceMock
                .Setup(c => c.GetControllerMethodsLinkedToAFeature(featureType))
                .Returns(controllerActions);

            var newControllers = controllerActions.Select(ca => ca.Controller).Except(ControllersAddedToFeatures).ToArray();

            ControllersAddedToFeatures.AddRange(newControllers);
            QualifiedEndpointsAdded.AddRange(controllerActions.Where(ca => !QualifiedEndpointsAdded.Contains(ca)).ToArray());

            return this;
        }

        public FeatureCacheFactoryTestFixtures WithExplicitFeatures(Feature[] explicitFeatures)
        {
            _explicitFeatures = explicitFeatures;
            return this;
        }

        public FeatureCacheFactory CreateFeatureToggleCacheFactory()
        {
            return new FeatureCacheFactory(ControllerMetaDataService);
        }

        public List<string> ControllersAddedToFeatures { get;}

        public List<ControllerAction> QualifiedEndpointsAdded { get; }
    }
}
