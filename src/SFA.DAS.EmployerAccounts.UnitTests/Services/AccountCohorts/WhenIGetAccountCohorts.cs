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
using SFA.DAS.EmployerAccounts.Models.Commitments;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services.AccountCohorts
{
    public class WhenIGetAccountCohorts
    {
        private Mock<ICommitmentsApiClient> _mockCommitmentsApiClient;
        private Mock<IEncodingService> _mockEncodingService;
        private Mock<IMapper> _mockMapper;
        private long _accountId = 123;
        private CommitmentsV2Service _sut;       

        [SetUp]
        public void Arrange()
        {
            _mockMapper = new Mock<IMapper>();
            _mockCommitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _mockEncodingService = new Mock<IEncodingService>();
            _sut = new CommitmentsV2Service(_mockCommitmentsApiClient.Object, _mockMapper.Object, _mockEncodingService.Object);
        }


        [Test]
        public async Task ThenGetApprenticeshipsResponse()
        {
            //Arrange
            _mockCommitmentsApiClient
                .Setup(c => c.GetApprenticeships(It.Is<GetApprenticeshipsRequest>(r => r.AccountId == _accountId), CancellationToken.None))
                .Returns(Task.FromResult(CreateApprenticeshipResponse()));
            var apprenticeships = new List<Apprenticeship>  { new Apprenticeship { ApprenticeshipStatus = EmployerAccounts.Models.Commitments.ApprenticeshipStatus.Approved,  FirstName ="FirstName" , LastName = "LastName" } };
            _mockMapper
             .Setup(m => m.Map<IEnumerable<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>, ICollection<Apprenticeship>>(It.IsAny<IEnumerable<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>>()))
             .Returns(apprenticeships);

            //Act
            var result =await _sut.GetApprenticeships(_accountId);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count().Equals(1));
        }

        


        [Test]
        public async Task ThenGetCohortsResponse()
        {
            //Arrange
            _mockCommitmentsApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.AccountId == _accountId), CancellationToken.None))
               .Returns(Task.FromResult(GetCohortsResponseForWithTrainingProviderStaus()));
            _mockEncodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns((long y, EncodingType z) => y + "_Encoded");

            var cohorts = new List<CohortV2>()
            {
                new CohortV2 { Id = 1, HashedId = "CohortHashedId" }
            };

            _mockMapper.Setup(m => m.Map<IEnumerable<CohortSummary>, IEnumerable<CohortV2>>(It.IsAny<CohortSummary[]>(), 
                It.IsAny<Action<IMappingOperationOptions<IEnumerable<CohortSummary>, IEnumerable<CohortV2>>>>()))
            .Returns(cohorts);

            //Act
            var result = await _sut.GetCohortsV2(_accountId);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count().Equals(1));
        }

        [Test]
        public async Task ThenGetDraftApprenticeshipsResponse()
        {
            //Arrange
            _mockCommitmentsApiClient.Setup(c => c.GetDraftApprenticeships(123, It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetDraftApprenticeshipsResponse()));
            var apprenticeships = new List<Apprenticeship> { new Apprenticeship { FirstName = "FirstName", LastName = "LastName" } };
            _mockMapper.Setup(m => m.Map<IEnumerable<DraftApprenticeshipDto>, List<Apprenticeship>>(It.IsAny<IReadOnlyCollection<DraftApprenticeshipDto>>(),
               It.IsAny<Action<IMappingOperationOptions<IEnumerable<DraftApprenticeshipDto>, List<Apprenticeship>>>>()))
           .Returns(apprenticeships);


            //Act
            var result = await _sut.GetDraftApprenticeships(123);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count().Equals(1));
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
