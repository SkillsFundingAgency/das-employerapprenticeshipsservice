using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.FeatureToggle;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Caching;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.FeatureToggleFileBasedServiceTests
{
    public class WhenIGetDisabledFeatures
    {
        private Mock<ICacheProvider> _cacheProvider;
        private FeatureToggleService _featureToggleService;
        private Mock<FeatureToggleService> _mockFeatureToggleFileBasedService;

        [SetUp]
        public void Arrange()
        {
            _cacheProvider = new Mock<ICacheProvider>();
            _cacheProvider.SetupSequence(c => c.Get<FeatureToggleLookup>(nameof(FeatureToggleLookup)))
                .Returns(null)
                .Returns(new FeatureToggleLookup());

            _mockFeatureToggleFileBasedService = new Mock<FeatureToggleService>(_cacheProvider.Object);
            _mockFeatureToggleFileBasedService.Setup(x => x.GetDataFromStorage()).Returns(new FeatureToggleLookup());
            _mockFeatureToggleFileBasedService.Setup(x => x.GetFeatures()).CallBase();
            _featureToggleService = _mockFeatureToggleFileBasedService.Object;
        }

        [Test]
        public void ThenTheFeaturesAreReadFromTheCacheOnSubsequentTimes()
        {
            //Act
            _featureToggleService.GetFeatures();
            _featureToggleService.GetFeatures();

            //Assert
            _mockFeatureToggleFileBasedService.Verify(x=>x.GetDataFromStorage(), Times.Once());
            _cacheProvider.Verify(x=>x.Get<FeatureToggleLookup>(nameof(FeatureToggleLookup)), Times.Exactly(2));
        }

        [Test]
        public void ThenTheValueIsNotAddedToTheCacheIfNullOrEmpty()
        {
            //Arrange
            _mockFeatureToggleFileBasedService.Setup(x => x.GetDataFromStorage()).Returns(new FeatureToggleLookup());

            //Act
            _featureToggleService.GetFeatures();

            //Assert
            _cacheProvider.Verify(x => x.Set(nameof(FeatureToggleLookup),It.IsAny<FeatureToggleLookup>(), It.IsAny<TimeSpan>()), Times.Never);
        }
    }
}
