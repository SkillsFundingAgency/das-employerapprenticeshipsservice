using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Requests.Reservations;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Responses.Reservations;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services.Reservations
{
    class WhenIGetReservationData
    {
        private Mock<IOuterApiClient> _mockOuterApiClient;
        private IReservationsService _reservationsService;
        private GetReservationsResponse _testData;
        long _accountId;

        [SetUp]
        public void Arrange()
        {
            _accountId = 123;

            _testData = new GetReservationsResponse
            {
                Reservations = new List<ReservationsResponse>
                {
                     new ReservationsResponse()
                    {
                        Id = new Guid(),
                        AccountId = _accountId,
                        Course = new ReservationCourse()
                        {
                            CourseId = "1"
                        }

                    }
                }
            };

            _mockOuterApiClient = new Mock<IOuterApiClient>();
            _mockOuterApiClient
                .Setup(m => m.Get<GetReservationsResponse>(It.Is<GetReservationsRequest>(k=>k.AccountId == _accountId)))
                .ReturnsAsync(_testData);

            _reservationsService = new ReservationsService(_mockOuterApiClient.Object, Mock.Of<ILogger<ReservationsService>>());
        }

        [Test]
        public  async Task ThenTheReservationsAreReturnedFromTheApi()
        {
            // arrange

            // act
            var result = await _reservationsService.Get(_accountId);

            // assert            
            result.Count().Should().Be(1);
            result.First().AccountId.Should().Be(_accountId);
        }
    }
}
