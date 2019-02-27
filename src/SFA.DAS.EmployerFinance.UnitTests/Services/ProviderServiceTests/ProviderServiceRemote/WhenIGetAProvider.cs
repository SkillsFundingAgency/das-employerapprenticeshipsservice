using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Providers.Api.Client;

namespace SFA.DAS.EmployerFinance.UnitTests.Services.ProviderServiceTests.ProviderServiceRemote
{
    internal class WhenIGetAProvider
    {
        private EmployerFinance.Services.ProviderServiceRemote _sut;
        private Mock<IProviderService> _mockProviderService;
        private Mock<IProviderApiClient> _mockProviderApiClient;
        private Mock<ILog> _mockLog;

        private string _providerName;

        [SetUp]
        public void Arrange()
        {
            _mockProviderService = new Mock<IProviderService>();
            _mockProviderApiClient = new Mock<IProviderApiClient>();
            _mockLog = new Mock<ILog>();

            _providerName = Guid.NewGuid().ToString();

            _mockProviderApiClient
                .Setup(m => m.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new Apprenticeships.Api.Types.Providers.Provider { ProviderName = _providerName });

            _mockLog
                .Setup(m => m.Warn(It.IsAny<Exception>(), It.IsAny<string>()))
                .Verifiable();

            _sut = new EmployerFinance.Services.ProviderServiceRemote(_mockProviderService.Object, _mockProviderApiClient.Object, _mockLog.Object);
        }

        [Test]
        public async Task ThenTheProviderIsRetrievedFromTheRemoteRepository()
        {
            // arrange 
            long ukPrn = RandomNumber(1, 100);

            // act
            var result = await _sut.Get(ukPrn);

            // assert
            _mockProviderApiClient.Verify(m => m.GetAsync(ukPrn), Times.Once);
            _mockProviderService.Verify(m => m.Get(ukPrn), Times.Never);
        }

        [Test]
        public async Task AndTheProviderExistsInTheRemoteRepository_ThenAValidProviderIsReturned()
        {
            // arrange 
            long ukPrn = RandomNumber(1, 100);

            // act
            var result = await _sut.Get(ukPrn);

            // assert
            Assert.AreEqual(_providerName, result.ProviderName);
        }

        [Test]
        public async Task AndTheProviderIsNotInTheRemoteRepository_ThenTheProviderServiceIsCalled()
        {
            // arrange
            long ukPrn = RandomNumber(1, 100);

            _mockProviderApiClient
                .Setup(m => m.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult< Apprenticeships.Api.Types.Providers.Provider>(null));

            // act
            var result = await _sut.Get(ukPrn);

            // assert
            _mockProviderService.Verify(m => m.Get(ukPrn), Times.Once);
        }

        [Test]
        public async Task AndTheRemoteRepositoryThrowsAnError_ThenTheErrorIsLoggedAsAWarning()
        {
            // arrange
            long ukPrn = RandomNumber(1, 100);
            var exception = new Exception();

            _mockProviderApiClient
                .Setup(m => m.GetAsync(ukPrn))
                .ThrowsAsync(exception);

            // act
            var result = await _sut.Get(ukPrn);

            // assert
            _mockLog.Verify(m => m.Warn(exception, $"Unable to get provider details with UKPRN {ukPrn} from apprenticeship API."), Times.Once);
        }

        [Test]
        public async Task AndTheRemoteRepositoryThrowsAnError_ThenTheProviderServiceIsCalled()
        {
            // arrange
            long ukPrn = RandomNumber(1, 100);
            var exception = new Exception();

            _mockProviderApiClient
                .Setup(m => m.GetAsync(ukPrn))
                .ThrowsAsync(exception);

            // act
            var result = await _sut.Get(ukPrn);

            // assert
            _mockProviderService.Verify(m => m.Get(ukPrn), Times.Once);
        }

        private int RandomNumber(int min, int max)
        {
            var random = new Random();
            return random.Next(min, max);
        }
    }
}