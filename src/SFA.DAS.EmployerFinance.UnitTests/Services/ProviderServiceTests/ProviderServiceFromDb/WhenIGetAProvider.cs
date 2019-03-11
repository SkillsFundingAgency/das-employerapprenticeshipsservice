using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;

namespace SFA.DAS.EmployerFinance.UnitTests.Services.ProviderServiceTests.ProviderServiceFromDb
{
    internal class WhenIGetAProvider
    {
        private EmployerFinance.Services.ProviderServiceFromDb _sut;
        private Mock<IDasLevyRepository> _mockDasLevyRepository;

        private string _providerName;

        [SetUp]
        public void Arrange()
        {
            _providerName = Guid.NewGuid().ToString();

            _mockDasLevyRepository = new Mock<IDasLevyRepository>();

            _mockDasLevyRepository
                .Setup(m => m.FindHistoricalProviderName(It.IsAny<long>()))
                .ReturnsAsync(new List<string>{_providerName});

            _sut = new EmployerFinance.Services.ProviderServiceFromDb(_mockDasLevyRepository.Object);
        }

        [Test]
        public async Task ThenTheHistoricalProviderNameIsRetrievedFromTheRepository()
        {
            // arrange 
            long ukPrn = 1234567890;           

            // act
            var result = await _sut.Get(ukPrn);

            // assert
            _mockDasLevyRepository.Verify(m => m.FindHistoricalProviderName(ukPrn), Times.Once);
        }

        [Test]
        public async Task AndTheHistoricalProviderNameExistsInTheDB_ThenAValidProviderIsReturned()
        {
            // arrange 
            long ukPrn = 1234567890;

            // act
            var result = await _sut.Get(ukPrn);

            // assert
            Assert.AreEqual(_providerName, result.Name);
            Assert.AreEqual(ukPrn, result.Ukprn);
            Assert.AreEqual(true, result.HistoricProviderName);
        }
    }
}