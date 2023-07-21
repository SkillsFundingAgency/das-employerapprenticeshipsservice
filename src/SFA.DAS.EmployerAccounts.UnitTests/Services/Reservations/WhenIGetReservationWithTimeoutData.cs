using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Polly;
using Polly.Registry;
using Polly.Timeout;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Responses.Reservations;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Reservations;
using SFA.DAS.EmployerAccounts.Services;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services.Reservations
{
    public class WhenIGetReservationWithTimeoutData
    {
        private ReservationsServiceWithTimeout _reservationsServiceWithTimeout;
        private Mock<IReservationsService> _reservationsService;
        private string _testData;
        private IAsyncPolicy _policy;
        private IEnumerable<Reservation> _reservations;
        long _accountId;

        [SetUp]
        public void Arrange()
        {
            _accountId = 123;
            _testData = JsonConvert.SerializeObject(new List<Reservation> { new Reservation { AccountId = _accountId } });
            _reservations = JsonConvert.DeserializeObject<IEnumerable<Reservation>>(_testData);

            var apiResponse = new GetReservationsResponse
            {
                Reservations = new List<ReservationsResponse>
                {
                    new ReservationsResponse()
                    {
                        Id = new Guid(),
                        Course = new ReservationCourse()
                        {
                            CourseId = "1"
                        }

                    }
                }
            };

            _policy = Policy.NoOpAsync();
            var registryPolicy = new PolicyRegistry();
            registryPolicy.Add(Constants.DefaultServiceTimeout, _policy);

            _reservationsService = new Mock<IReservationsService>();
            _reservationsService
                .Setup(rs => rs.Get(_accountId))
                .ReturnsAsync(_reservations);

            _reservationsServiceWithTimeout = new ReservationsServiceWithTimeout(_reservationsService.Object, registryPolicy);
        }

        [Test]
        public async Task ThenTheReservationsServiceIsCalled()
        {
            //act
            await _reservationsServiceWithTimeout.Get(_accountId);

            // assert 
            _reservationsService.Verify(rs => rs.Get(_accountId), Times.Once);
        }

        [Test]
        public async Task ThenTheReservationsServiceReturnsTheSameReservation()
        {
            //act
            var reservationsResult = await _reservationsServiceWithTimeout.Get(_accountId);

            // assert 
            Assert.AreSame(reservationsResult, _reservations);
        }

        [Test]
        public async Task ThenThrowTimeoutException()
        {
            var innerException = "Exception of type 'Polly.Timeout.TimeoutRejectedException' was thrown.";
            var message = "Call to Reservation Service timed out";
            Exception actualException = null;
            var correctExceptionThrown = false;

            try
            {
                _reservationsService.Setup(p => p.Get(_accountId))
                    .Throws<TimeoutRejectedException>();
                await _reservationsServiceWithTimeout.Get(_accountId);
            }
            catch (Exception e)
            {
                actualException = e;
                correctExceptionThrown = true;
            }
            Assert.IsTrue(correctExceptionThrown);
            Assert.AreEqual(actualException.InnerException?.Message, innerException);
            Assert.AreEqual(actualException.Message, message);
        }
    }
}
