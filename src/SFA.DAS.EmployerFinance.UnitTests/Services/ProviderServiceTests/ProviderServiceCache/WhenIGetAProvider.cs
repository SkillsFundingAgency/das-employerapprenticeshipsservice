using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Caches;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EmployerFinance.UnitTests.Services.ProviderServiceTests.ProviderServiceCache
{
    internal class WhenIGetAProvider
    {
        private EmployerFinance.Services.ProviderServiceCache _sut;
        private Mock<IProviderService> _mockProviderService;
        private Mock<IInProcessCache> _mockInProcessCache;

        private EmployerFinance.Models.ApprenticeshipProvider.Provider _testProvider;

        [SetUp]
        public void Arrange()
        {
            _mockProviderService = new Mock<IProviderService>();
            _mockInProcessCache = new Mock<IInProcessCache>();

            _testProvider = new EmployerFinance.Models.ApprenticeshipProvider.Provider();

            _mockInProcessCache
                .Setup(m => m.Get<EmployerFinance.Models.ApprenticeshipProvider.Provider>(It.IsAny<string>()))
                .Returns<EmployerFinance.Models.ApprenticeshipProvider.Provider>(null);

            _sut = new EmployerFinance.Services.ProviderServiceCache(_mockProviderService.Object, _mockInProcessCache.Object);
        }

        [Test]
        public async Task AndTheProviderIsInTheCache_ThenTheCachedProviderIsReturned()
        {
            // arrange 
            long ukPrn = 1234567890;

            _mockInProcessCache
                .Setup(m => m.Get<EmployerFinance.Models.ApprenticeshipProvider.Provider>($"{nameof(EmployerFinance.Models.ApprenticeshipProvider.Provider)}_{ukPrn}"))
                .Returns(_testProvider);

            // act
            var result = await _sut.Get(ukPrn);

            // assert
            Assert.AreEqual(_testProvider, result);
        }

        [Test]
        public async Task AndTheProviderIsInTheCache_ThenTheCacheIsQueriedOnce()
        {
            // arrange 
            long ukPrn = 1234567890;

            _mockInProcessCache
               .Setup(m => m.Get<EmployerFinance.Models.ApprenticeshipProvider.Provider>(It.IsAny<string>()))
               .Returns(_testProvider);

            // act
            var result = await _sut.Get(ukPrn);

            // assert
            _mockInProcessCache.Verify(m => m.Get<EmployerFinance.Models.ApprenticeshipProvider.Provider>($"{nameof(EmployerFinance.Models.ApprenticeshipProvider.Provider)}_{ukPrn}"), Times.Once);
            _mockInProcessCache.Verify(m => m.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()), Times.Never);
            _mockProviderService.Verify(m => m.Get(ukPrn), Times.Never);
        }

        [Test]
        public async Task AndTheProviderIsNotInTheCache_ThenTheProviderServiceIsCalled()
        {
            // arrange
            long ukPrn = 1234567890;

            _mockInProcessCache
                .Setup(m => m.Get<EmployerFinance.Models.ApprenticeshipProvider.Provider>(It.IsAny<string>()))
                .Returns<EmployerFinance.Models.ApprenticeshipProvider.Provider>(null);

            // act
            var result = await _sut.Get(ukPrn);

            // assert
            _mockProviderService.Verify(m => m.Get(ukPrn), Times.Once);
        }

        [Test]
        public async Task AndTheProviderServiceReturnsAProvider_ThenTheProviderIsStoredInTheCache()
        {
            // arrange
            long ukPrn = 1234567890;

            _mockProviderService
                .Setup(m => m.Get(ukPrn))
                .ReturnsAsync(_testProvider);

            // act
            var result = await _sut.Get(ukPrn);

            // assert
            _mockInProcessCache.Verify(m => m.Set($"{nameof(EmployerFinance.Models.ApprenticeshipProvider.Provider)}_{ukPrn}", _testProvider, It.IsAny<DateTimeOffset>()), Times.Once);
        }

        [Test]
        public async Task AndTheProviderServiceReturnsANullProvider_ThenTheProviderIsNotStoredInTheCache()
        {
            // arrange
            long ukPrn = 1234567890;

            _mockProviderService
                .Setup(m => m.Get(ukPrn))
                .Returns(Task.FromResult<EmployerFinance.Models.ApprenticeshipProvider.Provider>(null));

            // act
            var result = await _sut.Get(ukPrn);

            // assert
            _mockInProcessCache.Verify(m => m.Set($"{nameof(EmployerFinance.Models.ApprenticeshipProvider.Provider)}_{ukPrn}", It.IsAny<EmployerFinance.Models.ApprenticeshipProvider.Provider>()), Times.Never);
        }

        [Test]
        public async Task AndTheProviderServiceReturnsANullProvider_ThenTheNullProviderisReturned()
        {
            // arrange
            long ukPrn = 1234567890;

            _mockProviderService
                .Setup(m => m.Get(ukPrn))
                .Returns(Task.FromResult<EmployerFinance.Models.ApprenticeshipProvider.Provider>(null));

            // act
            var result = await _sut.Get(ukPrn);

            // assert
            Assert.IsNull(result);
        }
    }
}