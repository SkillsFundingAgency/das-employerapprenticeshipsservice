using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Reservations;
using SFA.DAS.EmployerAccounts.Services;
using FluentAssertions;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services.ReservationsApiClient
{
    class WhenIGetReservationData
    {
        private Mock<IReservationsApiClient> _mockReservationsApiClient;
        private IReservationsService _sut;
        private string _testData;

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

            _sut = new ReservationsService(_mockReservationsApiClient.Object);
        }

        [Test]
        public  async Task ThenTheReservationsAreReturnedFromTheApi()
        {
            // arrange

            // act
            var result = await _sut.Get(_accountId);

            // assert            
            result.Count().Should().Be(1);
            result.First().AccountId.Should().Be(_accountId);
        }
    }
}
