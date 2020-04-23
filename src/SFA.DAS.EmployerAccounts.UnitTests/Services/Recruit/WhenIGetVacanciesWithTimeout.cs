using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Polly;
using Polly.Registry;
using Polly.Timeout;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Recruit;
using SFA.DAS.EmployerAccounts.Services;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services.Recruit
{
    public class WhenIGetVacanciesWithTimeout
    {
        private Mock<IRecruitService> _mockRecruitService;
        private IAsyncPolicy _policy;
        private string _hashedAccountId;
        private RecruitServiceWithTimeout _recruitServiceWithTimeout;
        private IEnumerable<Vacancy> _vacancies = new List<Vacancy>()
        {
             new Vacancy()
            {
                Title = "Vacancy 1",
            },
             new Vacancy()
             {
                 Title = "Vacancy 2"
             }
        };

        [SetUp]
        public void Arrange()
        {
            _hashedAccountId = Guid.NewGuid().ToString();
            _mockRecruitService = new Mock<IRecruitService>();

            _mockRecruitService.Setup(rs => rs.GetVacancies(_hashedAccountId, int.MaxValue))
                .ReturnsAsync(_vacancies);
                
            _policy = Policy.NoOpAsync();
            var registryPolicy = new PolicyRegistry();
            registryPolicy.Add(Constants.DefaultServiceTimeout, _policy);

            _recruitServiceWithTimeout = new RecruitServiceWithTimeout(_mockRecruitService.Object, registryPolicy);
        }

        [Test]
        public async Task ThenTheRecruitServiceIsCalled()
        {
            //act
            await _recruitServiceWithTimeout.GetVacancies(_hashedAccountId);

            // assert 
            _mockRecruitService.Verify(x => x.GetVacancies(_hashedAccountId,int.MaxValue), Times.Once);
        }

        [Test]
        public async Task ThenTheRecruitServiceReturnsTheSameReservation()
        {
            //act
            var recruitsResult = await _recruitServiceWithTimeout.GetVacancies(_hashedAccountId);

            // assert 
            Assert.AreSame(recruitsResult, _vacancies);
        }

        [Test]
        public async Task ThenThrowTimeoutException()
        {
            var innerException = "Exception of type 'Polly.Timeout.TimeoutRejectedException' was thrown.";
            var message = "Call to Recruit Service timed out";
            Exception actualException = null;
            var correctExceptionThrown = false;

            try
            {
                _mockRecruitService.Setup(p => p.GetVacancies(_hashedAccountId, int.MaxValue))
                    .Throws<TimeoutRejectedException>();
                await _recruitServiceWithTimeout.GetVacancies(_hashedAccountId);
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
