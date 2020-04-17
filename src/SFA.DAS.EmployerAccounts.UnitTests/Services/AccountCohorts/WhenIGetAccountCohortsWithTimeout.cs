using SFA.DAS.CommitmentsV2.Api.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using System.Threading;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.CommitmentsV2.Types.Dtos;
using System.Linq;
using Polly;
using Polly.Registry;
using Polly.Timeout;
using SFA.DAS.EmployerAccounts.Models.CommitmentsV2;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services.AccountCohorts
{
    public class WhenIGetAccountCohortsWithTimeout
    {
        private Mock<ICommitmentsApiClient> _mockCommitmentsApiClient;
        private CommitmentsV2ServiceWithTimeout _commitmentsV2ServiceWithTimeout;
        private Mock<IEncodingService> _mockEncodingService;
        private Mock<IMapper> _mockMapper;
        private long _accountId = 123;
        private IAsyncPolicy _policy;

        [SetUp]
        public void Arrange()
        {
            _mockMapper = new Mock<IMapper>();
            _mockCommitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _mockEncodingService = new Mock<IEncodingService>();
            _policy = Policy.NoOpAsync();
            var registryPolicy = new PolicyRegistry();
            registryPolicy.Add(Constants.DefaultServiceTimeout, _policy);
            _mockCommitmentsApiClient
                .Setup(c => c.GetApprenticeships(It.Is<GetApprenticeshipsRequest>(r => r.AccountId == _accountId), CancellationToken.None))
                .Returns(Task.FromResult(CreateApprenticeshipResponse()));

            _mockCommitmentsApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.AccountId == _accountId), CancellationToken.None))
                .Returns(Task.FromResult(GetCohortsResponseForWithTrainingProviderStaus()));

            _mockCommitmentsApiClient.Setup(c => c.GetDraftApprenticeships(123, It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetDraftApprenticeshipsResponse()));

            _commitmentsV2ServiceWithTimeout = new CommitmentsV2ServiceWithTimeout(_mockCommitmentsApiClient.Object, _mockMapper.Object, _mockEncodingService.Object, registryPolicy);
        }

        [Test]
        public async Task ThenGetApprenticeshipsResponse()
        {
            //Arrange
           var apprenticeships = new List<Apprenticeship>  { new Apprenticeship { ApprenticeshipStatus = EmployerAccounts.Models.CommitmentsV2.ApprenticeshipStatus.Approved,  FirstName ="FirstName" , LastName = "LastName" } };

            _mockMapper
             .Setup(m => m.Map
             (It.IsAny<IEnumerable<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>>(),
             It.IsAny<Action<IMappingOperationOptions<IEnumerable<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>, IEnumerable<Apprenticeship>>>>()))
             .Returns(apprenticeships);

            //Act
            var result =await _commitmentsV2ServiceWithTimeout.GetApprenticeships(_accountId);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count().Equals(1));
        }

        [Test]
        public async Task ThenGetCohortsResponse()
        {
            //Arrange
            _mockEncodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns((long y, EncodingType z) => y + "_Encoded");

            var cohorts = new List<Cohort>()
            {
                new Cohort { Id = 1 }
            };

            _mockMapper.Setup(m => m.Map<IEnumerable<CohortSummary>, IEnumerable<Cohort>>(It.IsAny<CohortSummary[]>(), 
                It.IsAny<Action<IMappingOperationOptions<IEnumerable<CohortSummary>, IEnumerable<Cohort>>>>()))
            .Returns(cohorts);

            //Act
            var result = await _commitmentsV2ServiceWithTimeout.GetCohorts(_accountId);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count().Equals(1));
        }

        [Test]
        public async Task ThenGetDraftApprenticeshipsResponse()
        {
            //Arrange
            var apprenticeships = new List<Apprenticeship> { new Apprenticeship { FirstName = "FirstName", LastName = "LastName" } };
            _mockMapper.Setup(m => m.Map<IEnumerable<DraftApprenticeshipDto>, IEnumerable<Apprenticeship>>(It.IsAny<IReadOnlyCollection<DraftApprenticeshipDto>>(),
               It.IsAny<Action<IMappingOperationOptions<IEnumerable<DraftApprenticeshipDto>, IEnumerable<Apprenticeship>>>>()))
           .Returns(apprenticeships);


            //Act
            var result = await _commitmentsV2ServiceWithTimeout.GetDraftApprenticeships(new Cohort {Id = 123});

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count().Equals(1));
        }

        [Test]
        public async Task ThenGetApprenticeshipsIsCalled()
        {
            //act
            await _commitmentsV2ServiceWithTimeout.GetApprenticeships(_accountId);

            // assert 
            _mockCommitmentsApiClient.Verify(rs => rs.GetApprenticeships(It.Is<GetApprenticeshipsRequest>(r => r.AccountId == _accountId), CancellationToken.None), Times.Once());
        }

        [Test]
        public async Task ThenTheGetCohortsIsCalled()
        {
            //act
            await _commitmentsV2ServiceWithTimeout.GetCohorts(_accountId);

            // assert 
            _mockCommitmentsApiClient.Verify(rs => rs.GetCohorts(It.Is<GetCohortsRequest>(r => r.AccountId == _accountId), CancellationToken.None), Times.AtLeastOnce);
        }

        [Test]
        public async Task ThenTheGetDraftApprenticeshipsIsCalled()
        {
            //act
            await _commitmentsV2ServiceWithTimeout.GetDraftApprenticeships(new Cohort { Id = 123 });

            // assert 
            _mockCommitmentsApiClient.Verify(rs => rs.GetDraftApprenticeships(It.IsAny<long>(),CancellationToken.None), Times.AtLeastOnce);
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
                _mockCommitmentsApiClient.Setup(p => p.GetApprenticeships(It.Is<GetApprenticeshipsRequest>(r => r.AccountId == _accountId), CancellationToken.None))
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
                _mockCommitmentsApiClient.Setup(p => p.GetCohorts(It.Is<GetCohortsRequest>(r => r.AccountId == _accountId), CancellationToken.None))
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
                _mockCommitmentsApiClient.Setup(p => p.GetDraftApprenticeships(It.IsAny<long>(), CancellationToken.None))
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


        private GetApprenticeshipsResponse CreateApprenticeshipResponse()
        {
            IEnumerable<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse> apprenticeships = new List<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>()
            {
                new GetApprenticeshipsResponse.ApprenticeshipDetailsResponse
                {
                    Id = 1,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Uln = "Uln",
                    EmployerName = "EmployerName",
                    CourseName = "CourseName",
                    StartDate = new DateTime(2020, 5, 1),
                    EndDate = new DateTime(2022, 5, 1),
                    ApprenticeshipStatus = CommitmentsV2.Types.ApprenticeshipStatus.Live,
                    ProviderName = "ProviderName"
                }
            };

            return new GetApprenticeshipsResponse() { Apprenticeships = apprenticeships, TotalApprenticeships = 1, TotalApprenticeshipsFound = 1, TotalApprenticeshipsWithAlertsFound = 0 };
        }


        private GetCohortsResponse GetCohortsResponseForWithTrainingProviderStaus()
        {
            IEnumerable<CohortSummary> cohorts = new List<CohortSummary>()
            {
                new CohortSummary
                {
                    CohortId = 4,
                    AccountId = 1,
                    ProviderId = 4,
                    ProviderName = "Provider4",
                    NumberOfDraftApprentices = 1,
                    IsDraft = false,
                    WithParty = Party.Provider,
                    CreatedOn = DateTime.Now
                },
            };

            return new GetCohortsResponse(cohorts);
        }

        private GetDraftApprenticeshipsResponse GetDraftApprenticeshipsResponse()
        {
            IReadOnlyCollection<DraftApprenticeshipDto> draftApprenticeships = new List<DraftApprenticeshipDto>()
            {
                new DraftApprenticeshipDto
                {
                    Id = 1,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    DateOfBirth = new DateTime(2000, 1 ,1 ),
                    Cost = 100,
                    StartDate = new DateTime(2020, 5, 1),
                    EndDate = new DateTime(2022, 5, 1),
                    CourseCode = "CourseCode",
                    CourseName = "CourseName"
                }
            };

            var draftApprenticeshipsResponse = new GetDraftApprenticeshipsResponse() { DraftApprenticeships = draftApprenticeships };
            return draftApprenticeshipsResponse;
        }
    }
}
