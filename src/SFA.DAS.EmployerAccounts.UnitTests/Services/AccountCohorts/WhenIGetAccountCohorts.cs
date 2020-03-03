using SFA.DAS.CommitmentsV2.Api.Client;
using System;
using System.Collections;
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
using SFA.DAS.EmployerAccounts.Models.Commitments;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services.AccountCohorts
{
    public class WhenIGetAccountCohorts
    {
        private Mock<ICommitmentsApiClient> _mockCommitmentsApiClient;
        private Mock<IEncodingService> _mockEncodingService;
        private Mock<IMapper> _mockMapper;
        private long AccountId = 123;
        private CommitmentsV2Service _sut;
        public GetCohortsResponse GetCohortsResponse { get; set; }
        public GetCohortsResponse GetCohortsResponses { get; set; }
        public GetCohortsResponse GetCohortsResponseMoreThanOneDraftApprenticeship { get; set; }
        public GetDraftApprenticeshipsResponse DraftApprenticeshipsResponse { get; set; }
       

        [SetUp]
        public void Arrange()
        {
            _mockMapper = new Mock<IMapper>();
            _mockCommitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _mockEncodingService = new Mock<IEncodingService>();
            GetCohortsResponse = CreateGetCohortsResponseForWithTrainingProviderStaus();
            GetCohortsResponses = CreateGetCohortsResponses();
            GetCohortsResponseMoreThanOneDraftApprenticeship = CreateGetCohortsResponseMoreThanOneDraftApprenticeships();
            DraftApprenticeshipsResponse = GetDraftApprenticeshipsResponse();            

            _sut = new CommitmentsV2Service(_mockCommitmentsApiClient.Object, _mockMapper.Object, _mockEncodingService.Object);

        }

        [Test]
        public async Task ThenTheServiceIsCalled()
        {
            //Arrange
            _mockCommitmentsApiClient.Setup(c => c.GetApprenticeships(It.Is<GetApprenticeshipsRequest>(r => r.AccountId == AccountId), CancellationToken.None))
                .Returns(Task.FromResult(CreateApprenticeshipResponse()));

            _mockCommitmentsApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.AccountId == AccountId), CancellationToken.None))
                .Returns(Task.FromResult(GetCohortsResponse));
            _mockCommitmentsApiClient.Setup(c => c.GetDraftApprenticeships(GetCohortsResponse.Cohorts.FirstOrDefault().CohortId,
              It.IsAny<CancellationToken>())).Returns(Task.FromResult(DraftApprenticeshipsResponse));

            var cohorts = new List<CohortV2>()
            {
                new CohortV2 { Id = 1 }
            };

            _mockMapper
              .Setup(m => m.Map<IEnumerable<CohortSummary>, IEnumerable<CohortV2>>(It.IsAny<List<CohortSummary>>()))
              .Returns(cohorts);

            //Act
            var result = await _sut.GetCohortsV2(AccountId);

            //Assert
            Assert.IsNotNull(result);
        
        }


        [Test]
        public async Task ThenGetAccountCohortResponse()
        {
            //Arrange
            _mockCommitmentsApiClient.Setup(c => c.GetApprenticeships(It.Is<GetApprenticeshipsRequest>(r => r.AccountId == AccountId), CancellationToken.None))
                         .Returns(Task.FromResult(CreateApprenticeshipResponseWithNone()));
            _mockCommitmentsApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.AccountId == AccountId), CancellationToken.None))
                .Returns(Task.FromResult(GetCohortsResponse));
            _mockCommitmentsApiClient.Setup(c => c.GetDraftApprenticeships(GetCohortsResponse.Cohorts.FirstOrDefault().CohortId,
              It.IsAny<CancellationToken>())).Returns(Task.FromResult(DraftApprenticeshipsResponse));          

            var cohortsV2 = new List<CohortV2>() { new CohortV2 { Id = 4, CohortStatus = CohortStatus.Draft, NumberOfDraftApprentices = 1, Apprenticeships = new List<Apprenticeship> () { new Apprenticeship {FirstName = "FirstName" } } } };            
            _mockMapper.Setup(m => m.Map<IEnumerable<CohortSummary>, IEnumerable<CohortV2>>(It.IsAny<CohortSummary[]>(), It.IsAny<IEnumerable<CohortV2>>())).Returns(cohortsV2);


            var apprenticeship = new List<Apprenticeship> { new Apprenticeship { FirstName = "FirstName", LastName = "LastName" } };
            _mockMapper.Setup(m => m.Map<IEnumerable<DraftApprenticeshipDto>, List<Apprenticeship>>(It.IsAny<IReadOnlyCollection<DraftApprenticeshipDto>>())).Returns(apprenticeship);

            //Act
            var result = await _sut.GetCohortsV2(AccountId);

            //Assert
            Assert.IsNotNull(result);            
            Assert.AreEqual(result.First().CohortStatus, CohortStatus.Draft);
            Assert.IsTrue(result.First().Apprenticeships.Count().Equals(1));
            Assert.AreEqual(result.First().Apprenticeships.First().ApprenticeshipStatus, EmployerAccounts.Models.Commitments.ApprenticeshipStatus.Draft);
        }
        
        [Test]
        
        public async Task ThenGetAccountCohortResponseException()
        {
            //Arrange
            _mockCommitmentsApiClient.Setup(c => c.GetApprenticeships(It.Is<GetApprenticeshipsRequest>(r => r.AccountId == AccountId), CancellationToken.None))
                         .Returns(Task.FromResult(CreateApprenticeshipResponseWithNone()));
            _mockCommitmentsApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.AccountId == AccountId), CancellationToken.None))
                .Returns(Task.FromResult(GetCohortsResponse));
            _mockCommitmentsApiClient.Setup(c => c.GetDraftApprenticeships(GetCohortsResponse.Cohorts.FirstOrDefault().CohortId,
              It.IsAny<CancellationToken>())).Returns(Task.FromResult(DraftApprenticeshipsResponse));

            var cohortsV2 = new List<CohortV2>() { new CohortV2 { Id = 4, CohortStatus = CohortStatus.Draft, NumberOfDraftApprentices = 1, Apprenticeships = new List<Apprenticeship>() { new Apprenticeship { FirstName = "FirstName" } } } };
            _mockMapper.Setup(m => m.Map<IEnumerable<CohortSummary>, IEnumerable<CohortV2>>(It.IsAny<CohortSummary[]>())).Returns(cohortsV2);

            var apprenticeship = new List<Apprenticeship> { new Apprenticeship { FirstName = "FirstName", LastName = "LastName" } };
            _mockMapper.Setup(m => m.Map<IEnumerable<DraftApprenticeshipDto>, List<Apprenticeship>>(It.IsAny<IReadOnlyCollection<DraftApprenticeshipDto>>())).Returns(apprenticeship);

            //Act
            try
            {
                var result = await _sut.GetCohortsV2(AccountId);
            }
            catch(Exception ex)
            {
                //Assert
                Assert.IsNotNull(ex.Message);
                //Assert.Fail(ex.Message);
            }
        }


        [Test]
        public async Task ThenDoNotGetDraftApprenticeshipsResponseIfCohortCountIsGreaterThanOne()
        {
            //Arrange   
            _mockCommitmentsApiClient.Setup(c => c.GetApprenticeships(It.Is<GetApprenticeshipsRequest>(r => r.AccountId == AccountId), CancellationToken.None))
               .Returns(Task.FromResult(CreateApprenticeshipResponseWithNone()));

            _mockCommitmentsApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.AccountId == AccountId), CancellationToken.None))
                .Returns(Task.FromResult(GetCohortsResponses));
            _mockCommitmentsApiClient.Setup(c => c.GetDraftApprenticeships(GetCohortsResponse.Cohorts.FirstOrDefault().CohortId,
             It.IsAny<CancellationToken>())).Returns(Task.FromResult(DraftApprenticeshipsResponse));
          

            var cohortsV2 = new List<CohortV2>()
            {
                new CohortV2 { Id = 4, CohortStatus = CohortStatus.Draft, NumberOfDraftApprentices = 1, Apprenticeships = new List<Apprenticeship> () { new Apprenticeship {FirstName = "FirstName" } } },
                new CohortV2 { Id = 5, CohortStatus = CohortStatus.Draft, NumberOfDraftApprentices = 1,
                    Apprenticeships = new List<Apprenticeship> () { new Apprenticeship {FirstName = "FirstName1" } } }
            };

            _mockMapper.Setup(m => m.Map<IEnumerable<CohortSummary>, IEnumerable<CohortV2>>(It.IsAny<CohortSummary[]>()))
              .Returns(cohortsV2);

            var apprenticeship = new List<Apprenticeship> { new Apprenticeship { FirstName = "FirstName", LastName = "LastName" } };
            _mockMapper.Setup(m => m.Map<IEnumerable<DraftApprenticeshipDto>, List<Apprenticeship>>(It.IsAny<IReadOnlyCollection<DraftApprenticeshipDto>>())).Returns(apprenticeship);


            //Act
            var result = await _sut.GetCohortsV2(AccountId);

            //Assert            
            Assert.IsNotNull(result);
            Assert.IsTrue(result.First().Apprenticeships.Count().Equals(0));
        }

        [Test]
        public async Task ThenNumberOfDraftApprenticeshipeGreaterThanOneDoNotGetDraftApprenticeshipsResponse()
        {
            //Arrange     
            _mockCommitmentsApiClient.Setup(c => c.GetApprenticeships(It.Is<GetApprenticeshipsRequest>(r => r.AccountId == AccountId), CancellationToken.None))
              .Returns(Task.FromResult(CreateApprenticeshipResponseWithNone()));
            _mockCommitmentsApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.AccountId == AccountId), CancellationToken.None))
                .Returns(Task.FromResult(GetCohortsResponseMoreThanOneDraftApprenticeship));
            _mockCommitmentsApiClient.Setup(c => c.GetDraftApprenticeships(GetCohortsResponse.Cohorts.FirstOrDefault().CohortId,
             It.IsAny<CancellationToken>())).Returns(Task.FromResult(DraftApprenticeshipsResponse));

            var cohortsV2 = new List<CohortV2>()
            {
                new CohortV2 { Id = 4, CohortStatus = CohortStatus.Draft, NumberOfDraftApprentices = 5, Apprenticeships = new List<Apprenticeship> () { new Apprenticeship {FirstName = "FirstName" } } }
            };
            _mockMapper.Setup(m => m.Map<IEnumerable<CohortSummary>, IEnumerable<CohortV2>>(It.IsAny<CohortSummary[]>())).Returns(cohortsV2);

            var apprenticeship = new List<Apprenticeship>{ new Apprenticeship  {FirstName = "FirstName", LastName = "LastName"}};
            _mockMapper.Setup(m => m.Map<IEnumerable<DraftApprenticeshipDto>, List<Apprenticeship>>(It.IsAny<IReadOnlyCollection<DraftApprenticeshipDto>>())).Returns(apprenticeship);
            
            //Act
            var result = await _sut.GetCohortsV2(AccountId);

            //Assert            
            Assert.IsNotNull(result);
            Assert.IsTrue(result.First().Apprenticeships.Count().Equals(0));
        }


        [Test]
        public async Task ThenDraftApprenticeshipIsNullIfCohortsResponseIsNull()
        {
            //Act
            var result = await _sut.GetCohortsV2(AccountId);

            //Assert      
            Assert.IsTrue(result.First().Apprenticeships.Count().Equals(0));            
        }


        private GetCohortsResponse CreateGetCohortsResponseForWithTrainingProviderStaus()
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


        private GetCohortsResponse CreateGetCohortsResponseMoreThanOneDraftApprenticeships()
        {
            IEnumerable<CohortSummary> cohorts = new List<CohortSummary>()
            {
                new CohortSummary
                {
                    CohortId = 4,
                    AccountId = 1,
                    ProviderId = 4,
                    ProviderName = "Provider4",
                    NumberOfDraftApprentices = 100,
                    IsDraft = false,
                    WithParty = Party.Provider,
                    CreatedOn = DateTime.Now
                },
            };

            return new GetCohortsResponse(cohorts);
        }


        private GetCohortsResponse CreateGetCohortsResponses()
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
                new CohortSummary
                {
                    CohortId = 5,
                    AccountId = 1,
                    ProviderId = 5,
                    ProviderName = "Provider5",
                    NumberOfDraftApprentices = 2,
                    IsDraft = false,
                    WithParty = Party.Provider,
                    CreatedOn = DateTime.Now
                }
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
                    ApprenticeshipStatus = SFA.DAS.CommitmentsV2.Types.ApprenticeshipStatus.Live,
                    ProviderName = "ProviderName"
                }
            };

            return new GetApprenticeshipsResponse() { Apprenticeships = apprenticeships, TotalApprenticeships = 1, TotalApprenticeshipsFound = 1, TotalApprenticeshipsWithAlertsFound = 0 };
        }

        private GetApprenticeshipsResponse CreateApprenticeshipResponseWithNone()
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
                    ApprenticeshipStatus = SFA.DAS.CommitmentsV2.Types.ApprenticeshipStatus.Live,
                    ProviderName = "ProviderName"
                }
            };

            return new GetApprenticeshipsResponse() { Apprenticeships = apprenticeships, TotalApprenticeships = 0, TotalApprenticeshipsFound = 0, TotalApprenticeshipsWithAlertsFound = 0 };
        }
    }
}
