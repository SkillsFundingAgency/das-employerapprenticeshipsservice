using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Services;
using System.Linq;
using Polly;
using Polly.Registry;
using Polly.Timeout;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.CommitmentsV2;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services.AccountCohorts
{
    public class WhenIGetAccountCohortsWithTimeout
    {
        private Mock<ICommitmentV2Service> _mockCommitmentsV2Service;
        private CommitmentsV2ServiceWithTimeout _commitmentsV2ServiceWithTimeout;
        private long _accountId = 123;
        private IAsyncPolicy _policy;
        private List<Apprenticeship> _apprenticeships;
        private List<Cohort> _cohorts;
        private Cohort _cohort;

        [SetUp]
        public void Arrange()
        {
            _cohort=new Cohort(){Id = 123};
            _cohorts = new List<Cohort>
            {
                new Cohort() {Id = 123}
            };

            _apprenticeships = new List<Apprenticeship>
            {
                new Apprenticeship {ApprenticeshipStatus = ApprenticeshipStatus.Approved, FirstName = "FirstName", LastName = "LastName"}
            };
            
            _mockCommitmentsV2Service = new Mock<ICommitmentV2Service>();
            _policy = Policy.NoOpAsync();
            var registryPolicy = new PolicyRegistry();
            registryPolicy.Add(Constants.DefaultServiceTimeout, _policy);

            _mockCommitmentsV2Service
                .Setup(c => c.GetApprenticeships(_accountId))
                .ReturnsAsync(_apprenticeships);

            _mockCommitmentsV2Service
                .Setup(c => c.GetCohorts(_accountId))
                .ReturnsAsync(_cohorts);

            _mockCommitmentsV2Service
                .Setup(c => c.GetDraftApprenticeships(_cohort))
                .ReturnsAsync(_apprenticeships);

            _commitmentsV2ServiceWithTimeout = new CommitmentsV2ServiceWithTimeout(_mockCommitmentsV2Service.Object, registryPolicy);
        }

        [Test]
        public async Task ThenGetApprenticeshipsResponse()
        {
            //Act
            var result = await _commitmentsV2ServiceWithTimeout.GetApprenticeships(_accountId);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count().Equals(1));
        }

        [Test]
        public async Task ThenGetCohortsResponse()
        {
            //Act
            var result = await _commitmentsV2ServiceWithTimeout.GetCohorts(_accountId);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count().Equals(1));
        }

        [Test]
        public async Task ThenGetDraftApprenticeshipsResponse()
        {
            //Act
            var result = await _commitmentsV2ServiceWithTimeout.GetDraftApprenticeships(_cohort);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count().Equals(1));
        }

        [Test]
        public async Task ThenGetApprenticeshipsResponseReturnsTheSameApprenticeship()
        {
            //act
            var apprenticeshipsResult = await _commitmentsV2ServiceWithTimeout.GetApprenticeships(_accountId);

            // assert 
            Assert.AreSame(apprenticeshipsResult, _apprenticeships);
        }

        [Test]
        public async Task ThenGetCohortsResponseReturnsTheSameCohorts()
        {
            //act
            var cohortsResult = await _commitmentsV2ServiceWithTimeout.GetCohorts(_accountId);

            // assert 
            Assert.AreSame(cohortsResult, _cohorts);
        }

        [Test]
        public async Task ThenGetDraftApprenticeshipsResponseReturnsTheSameApprenticeship()
        {
            //act
            var apprenticeshipsResult = await _commitmentsV2ServiceWithTimeout.GetDraftApprenticeships(_cohort);

            // assert 
            Assert.AreSame(apprenticeshipsResult, _apprenticeships);
        }

        [Test]
        public async Task ThenGetApprenticeshipsIsCalled()
        {
            //act
            await _commitmentsV2ServiceWithTimeout.GetApprenticeships(_accountId);

            // assert 
            _mockCommitmentsV2Service.Verify(rs => rs.GetApprenticeships(_accountId), Times.Once());
        }

        [Test]
        public async Task ThenTheGetCohortsIsCalled()
        {
            //act
            await _commitmentsV2ServiceWithTimeout.GetCohorts(_accountId);

            // assert 
            _mockCommitmentsV2Service.Verify(rs => rs.GetCohorts(_accountId), Times.Once);
        }

        [Test]
        public async Task ThenTheGetDraftApprenticeshipsIsCalled()
        {
            //act
            await _commitmentsV2ServiceWithTimeout.GetDraftApprenticeships(_cohort);

            // assert 
            _mockCommitmentsV2Service.Verify(rs => rs.GetDraftApprenticeships(_cohort), Times.Once);
        }

        [Test]
        public async Task ThenThrowTimeoutException_GetApprenticeships()
        {
            var innerException = "Exception of type 'Polly.Timeout.TimeoutRejectedException' was thrown.";
            var message = "Call to Commitments V2 Service timed out";
            Exception actualException = null;
            var correctExceptionThrown = false;

            try
            {
                _mockCommitmentsV2Service
                    .Setup(c => c.GetApprenticeships(_accountId))
                    .Throws<TimeoutRejectedException>();
                await _commitmentsV2ServiceWithTimeout.GetApprenticeships(_accountId);
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

        [Test]
        public async Task ThenThrowTimeoutException_GetCohorts()
        {
            var innerException = "Exception of type 'Polly.Timeout.TimeoutRejectedException' was thrown.";
            var message = "Call to Commitments V2 Service timed out";
            Exception actualException = null;
            var correctExceptionThrown = false;

            try
            {
                _mockCommitmentsV2Service
                    .Setup(c => c.GetCohorts(_accountId))
                    .Throws<TimeoutRejectedException>();
                await _commitmentsV2ServiceWithTimeout.GetCohorts(_accountId);
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

        [Test]
        public async Task ThenThrowTimeoutException_GetDraftApprenticeships()
        {
            var innerException = "Exception of type 'Polly.Timeout.TimeoutRejectedException' was thrown.";
            var message = "Call to Commitments V2 Service timed out";
            Exception actualException = null;
            var correctExceptionThrown = false;

            try
            {
                _mockCommitmentsV2Service
                    .Setup(c => c.GetDraftApprenticeships(It.IsAny<Cohort>()))
                    .Throws<TimeoutRejectedException>();
                await _commitmentsV2ServiceWithTimeout.GetDraftApprenticeships(new Cohort { Id = 123 });
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
