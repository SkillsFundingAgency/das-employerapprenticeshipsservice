using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.CommitmentsV2.Types.Dtos;
using MediatR;
using SFA.DAS.EmployerAccounts.Queries.GetReservations;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.Row1Panel1OrchestratorTests
{
    public class WhenGettingDraftApprenticeship
    {

        private const string HashedAccountId = "ABC123";
        private const long AccountId = 123;
        private const string UserId = "USER1";
        private Row1Panel1Orchestrator row1Panel1Orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ICommitmentsApiClient> mockCommitmentApiClient;
        private Mock<IEncodingService> mockEncodingService;
        public GetCohortsResponse GetCohortsResponse { get; set; }
        public GetCohortsResponse GetCohortsResponses { get; set; }
        public GetCohortsResponse GetCohortsResponseMoreThanOneDraftApprenticeship { get; set; }
        public GetDraftApprenticeshipsResponse DraftApprenticeshipsResponse { get; set; }

        [SetUp]
        public void Arrange()
        {
            //Arrange
            _mediator = new Mock<IMediator>();
            mockCommitmentApiClient = new Mock<ICommitmentsApiClient>();
            mockEncodingService = new Mock<IEncodingService>();


            _mediator.Setup(m => m.SendAsync(It.Is<GetReservationsRequest>(q => q.HashedAccountId == HashedAccountId)))
                 .ReturnsAsync(new GetReservationsResponse
                 {
                     Reservations = new List<EmployerAccounts.Models.Reservations.Reservation>
                     {
                             new  EmployerAccounts.Models.Reservations.Reservation
                             {
                                 AccountId = 123
                             }
                     }
                 });


            GetCohortsResponse = CreateGetCohortsResponseForWithTrainingProviderStaus();
            GetCohortsResponses = CreateGetCohortsResponses();
            GetCohortsResponseMoreThanOneDraftApprenticeship = CreateGetCohortsResponseMoreThanOneDraftApprenticeships();
            // mockCommitmentApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.AccountId == AccountId), CancellationToken.None)).Returns(Task.FromResult(GetCohortsResponse));

            DraftApprenticeshipsResponse = GetDraftApprenticeshipsResponseTest();
            //mockCommitmentApiClient.Setup(c => c.GetDraftApprenticeships(GetCohortsResponse.Cohorts.FirstOrDefault().CohortId,
            //    It.IsAny<CancellationToken>())).Returns(Task.FromResult(DraftApprenticeshipsResponse));

            mockEncodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns((long y, EncodingType z) => y + "_Encoded");
            mockEncodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.ApprenticeshipId)).Returns((long y, EncodingType z) => y + "_Encoded");
            row1Panel1Orchestrator = new Row1Panel1Orchestrator(_mediator.Object,  mockCommitmentApiClient.Object, mockEncodingService.Object);
        }


        [Test]
        public async Task ThenShouldGetDraftApprenticeshipResponse()
        {
            //Arrange
            mockCommitmentApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.AccountId == AccountId), CancellationToken.None)).Returns(Task.FromResult(GetCohortsResponse));
            mockCommitmentApiClient.Setup(c => c.GetDraftApprenticeships(GetCohortsResponse.Cohorts.FirstOrDefault().CohortId,
               It.IsAny<CancellationToken>())).Returns(Task.FromResult(DraftApprenticeshipsResponse));

            //Act
            var result = await row1Panel1Orchestrator.GetAccount(HashedAccountId, AccountId, UserId);

            //Assert
            var expected = DraftApprenticeshipsResponse.DraftApprenticeships.First();
            Assert.IsTrue(result.Data.ApprenticeshipsCount.Equals(0));
            Assert.IsTrue(result.Data.CohortsCount.Equals(1));
            Assert.IsTrue(result.Data.CohortStatus.Equals(CohortStatus.WithTrainingProvider));
            Assert.IsTrue(result.Data.HasSingleDraftApprenticeship);
            Assert.AreEqual("1_Encoded", result.Data.HashedDraftApprenticeshipId);
            Assert.AreEqual("4_Encoded", result.Data.HashedCohortReference);
            Assert.AreEqual(expected.CourseName, result.Data.CourseName);
            Assert.AreEqual(expected.StartDate, result.Data.CourseStartDate);
            //Assert.AreEqual(expected.Select(s => s.EndDate).ToList()[0], result.Data.CourseEndDate);
            Assert.IsNotNull(result);
        }





        [Test]
        public void WrireVerifyTests()
        {

        }

        [Test]
        public async Task ThenDraftApprenticeshipIsNullWhenCohortsResponseIsNull()
        {
            //Act
            var result = await row1Panel1Orchestrator.GetAccount(HashedAccountId, AccountId, UserId);

            //Assert
            var expected = DraftApprenticeshipsResponse.DraftApprenticeships.Where(x => x.CourseName == "CourseName");
            Assert.IsTrue(result.Data.CohortsCount.Equals(0));
        }

        [Test]
        public async Task ThenGetDraftResponseWhenCohortCountIsOne()
        {
            //Arrange
            mockCommitmentApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.AccountId == AccountId), CancellationToken.None)).Returns(Task.FromResult(GetCohortsResponse));
            mockCommitmentApiClient.Setup(c => c.GetDraftApprenticeships(GetCohortsResponse.Cohorts.FirstOrDefault().CohortId,
               It.IsAny<CancellationToken>())).Returns(Task.FromResult(DraftApprenticeshipsResponse));

            //Act
            var result = await row1Panel1Orchestrator.GetAccount(HashedAccountId, AccountId, UserId);

            //Assert
            var expected = DraftApprenticeshipsResponse.DraftApprenticeships.First();
            Assert.IsTrue(result.Data.CohortsCount.Equals(1));
        }


        [Test]
        public async Task ThenDoNotGetDraftApprenticeshipsResponseWhenCohortCountIsGreaterThanOne()
        {
            //Arrange
            mockCommitmentApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.AccountId == AccountId), CancellationToken.None)).Returns(Task.FromResult(GetCohortsResponses));
            mockCommitmentApiClient.Setup(c => c.GetDraftApprenticeships(GetCohortsResponses.Cohorts.FirstOrDefault().CohortId,
               It.IsAny<CancellationToken>())).Returns(Task.FromResult(DraftApprenticeshipsResponse));

            //Act
            var result = await row1Panel1Orchestrator.GetAccount(HashedAccountId, AccountId, UserId);

            //Assert
            var expected = DraftApprenticeshipsResponse.DraftApprenticeships.First();
            Assert.AreEqual(result.Data.CohortsCount, 0);
        }


        [Test]
        public async Task  ThenNumberOfDraftApprenticeshipeGreaterThanOneDoNotGetDraftApprenticeshipsResponse()
        {
            //Arrange
            mockCommitmentApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.AccountId == AccountId), CancellationToken.None)).Returns(Task.FromResult(GetCohortsResponseMoreThanOneDraftApprenticeship));
            mockCommitmentApiClient.Setup(c => c.GetDraftApprenticeships(GetCohortsResponseMoreThanOneDraftApprenticeship.Cohorts.FirstOrDefault().CohortId,
               It.IsAny<CancellationToken>())).Returns(Task.FromResult(DraftApprenticeshipsResponse));

            //Act
            var result = await row1Panel1Orchestrator.GetAccount(HashedAccountId, AccountId, UserId);

            //Assert
            var expected = DraftApprenticeshipsResponse.DraftApprenticeships.First();
            Assert.AreEqual(result.Data.CohortsCount, 1);
            Assert.IsFalse(result.Data.HasSingleDraftApprenticeship);            
            Assert.AreEqual("4_Encoded", result.Data.HashedCohortReference);
            Assert.AreEqual(GetCohortsResponseMoreThanOneDraftApprenticeship.Cohorts.First().NumberOfDraftApprentices, result.Data.NumberOfDraftApprentices);
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

        private GetDraftApprenticeshipsResponse GetDraftApprenticeshipsResponseTest()
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
