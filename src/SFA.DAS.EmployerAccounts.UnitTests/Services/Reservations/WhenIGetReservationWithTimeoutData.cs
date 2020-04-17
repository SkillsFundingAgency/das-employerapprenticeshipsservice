using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Polly;
using Polly.Registry;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Reservations;
using SFA.DAS.EmployerAccounts.Services;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services.Reservations
{
    class WhenIGetReservationWithTimeoutData
    {
        private Mock<IReservationsApiClient> _mockReservationsApiClient;
        private IReservationsService _sut;
        private Mock<ReservationsService> _mockreservationsService;
        private string _testData;
        private Mock<IAsyncPolicy> _mockPolicy;

        long _accountId;

        [SetUp]
        public void Arrange()
        {
            _accountId = 123;
            _testData = JsonConvert.SerializeObject(new List<Reservation> { new Reservation { AccountId = _accountId } });

            _mockReservationsApiClient = new Mock<IReservationsApiClient>();
            _mockReservationsApiClient
                .Setup(m => m.Get(_accountId))
                .ReturnsAsync(_testData);
            _mockPolicy = new Mock<IAsyncPolicy>();
            var mockRegistryPolicy = new PolicyRegistry();
            mockRegistryPolicy.Add(Constants.DefaultServiceTimeout, _mockPolicy.Object);
            _mockreservationsService = new Mock<ReservationsService>(_mockReservationsApiClient.Object);
            _sut = new ReservationsServiceWithTimeout(_mockreservationsService.Object, mockRegistryPolicy);
        }

        [Test]
        public async Task ThenTheReservationsAreReturnedFromTheService()
        {
            //arrange
            IEnumerable<Reservation> reservations = new List<Reservation> {
                new Reservation {AccountId = _accountId}
            };
            //_mockreservationsService.Setup(rs => rs.Get(_accountId))
            //    .Returns(It.IsAny<Task<IEnumerable<Reservation>>>());
            //act
            await _sut.Get(_accountId);

            // assert 
            _mockPolicy.Verify(p => p.ExecuteAsync(It.IsAny<Func<Task<IEnumerable<Reservation>>>>()));
        }

        [Test]
        public async Task ThenTheReservationsServiceReturnsATimeout()
        {
            //act
            await _sut.Get(_accountId);

            // assert 
            _mockPolicy.Verify(p => p.ExecuteAsync(It.IsAny<Func<Task<IEnumerable<Reservation>>>>()));
        }
    }
}
