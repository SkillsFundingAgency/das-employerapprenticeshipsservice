using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests.Providers;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses.Providers;
using SFA.DAS.EmployerFinance.Interfaces.OuterApi;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.UnitTests.Services.ProviderServiceTests.ProviderServiceRemote
{
    internal class WhenIGetAProvider
    {
        private EmployerFinance.Services.ProviderServiceRemote _sut;
        private Mock<IProviderService> _mockProviderService;
        private Mock<IOuterApiClient> _mockApiClient;
        private Mock<ILog> _mockLog;

        private string _providerName;
        private GetProviderResponse _provider;

        [SetUp]
        public void Arrange()
        {
            
            _provider = new GetProviderResponse
            {
                Email = "test@test.com"
            };
            _mockProviderService = new Mock<IProviderService>();
            _mockApiClient = new Mock<IOuterApiClient>();
            _mockLog = new Mock<ILog>();

            _providerName = Guid.NewGuid().ToString();

            _mockApiClient
                .Setup(m => m.Get<GetProviderResponse>(It.IsAny<GetProviderRequest>()))
                .ReturnsAsync(_provider);

            _mockLog
                .Setup(m => m.Warn(It.IsAny<Exception>(), It.IsAny<string>()))
                .Verifiable();

            _sut = new EmployerFinance.Services.ProviderServiceRemote(_mockProviderService.Object, _mockApiClient.Object, _mockLog.Object);
        }

        [Test]
        public async Task ThenTheProviderIsRetrievedFromTheRemoteRepository()
        {
            // arrange 
            long ukPrn = 1234567890;

            // act
            var actual = await _sut.Get(ukPrn);

            // assert
            _mockApiClient.Verify(m => m.Get<GetProviderResponse>(It.Is<GetProviderRequest>(c=>c.GetUrl.Equals($"providers/{ukPrn}"))), Times.Once);
            _mockProviderService.Verify(m => m.Get(ukPrn), Times.Never);
            actual.ShouldBeEquivalentTo(_provider, options=>options
                .Excluding(c=>c.Id)
                .Excluding(c=>c.NationalProvider)
                .Excluding(c=>c.IsHistoricProviderName)
            );
        }


        [Test]
        public async Task AndTheProviderIsNotInTheRemoteRepository_ThenTheProviderServiceIsCalled()
        {
            // arrange
            long ukPrn = 1234567890;

            _mockApiClient
                .Setup(m => m.Get<GetProviderResponse>(It.IsAny<GetProviderRequest>()))
                .ReturnsAsync((GetProviderResponse)null);

            // act
            await _sut.Get(ukPrn);

            // assert
            _mockProviderService.Verify(m => m.Get(ukPrn), Times.Once);
        }

        [Test]
        public async Task AndTheRemoteRepositoryThrowsAnError_ThenTheErrorIsLoggedAsAWarning()
        {
            // arrange
            long ukPrn = 1234567890;
            var exception = new Exception();

            _mockApiClient
                .Setup(m => m.Get<GetProviderResponse>(It.IsAny<GetProviderRequest>()))
                .ThrowsAsync(exception);

            // act
            await _sut.Get(ukPrn);

            // assert
            _mockLog.Verify(m => m.Warn(exception, $"Unable to get provider details with UKPRN {ukPrn} from apprenticeship API."), Times.Once);
        }

        [Test]
        public async Task AndTheRemoteRepositoryThrowsAnError_ThenTheProviderServiceIsCalled()
        {
            // arrange
            long ukPrn = 1234567890;
            var exception = new Exception();

            _mockApiClient
                .Setup(m => m.Get<GetProviderResponse>(It.IsAny<GetProviderRequest>()))
                .ThrowsAsync(exception);

            // act
            await _sut.Get(ukPrn);

            // assert
            _mockProviderService.Verify(m => m.Get(ukPrn), Times.Once);
        }
    }
}