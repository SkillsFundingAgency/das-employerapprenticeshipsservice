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

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.Row1Panel1OrchestratorTests
{
    public class WhenGettingDraftApprenticeship
    {

        private const string HashedAccountId = "ABC123";
        private const long AccountId = 123;
        private Row1Panel1Orchestrator row1Panel1Orchestrator;
        private Mock<ICommitmentsApiClient> mockCommitmentApiClient;
        private Mock<IEncodingService> mockEncodingService;
        public GetCohortsResponse GetCohortsResponse { get; set; }
        public GetDraftApprenticeshipsResponse DraftApprenticeshipsResponse { get; set; }

        [SetUp]
        public void Arrange()
        {
            //Arrange
            mockCommitmentApiClient = new Mock<ICommitmentsApiClient>();
            mockEncodingService = new Mock<IEncodingService>();

            GetCohortsResponse = CreateGetCohortsResponseForWithTrainingProviderStaus();            
            mockCommitmentApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.AccountId == AccountId), CancellationToken.None)).Returns(Task.FromResult(GetCohortsResponse));

            DraftApprenticeshipsResponse = GetDraftApprenticeshipsResponseTest();
            mockCommitmentApiClient.Setup(c => c.GetDraftApprenticeships(GetCohortsResponse.Cohorts.FirstOrDefault().CohortId,
                It.IsAny<CancellationToken>())).Returns(Task.FromResult(DraftApprenticeshipsResponse));

            mockEncodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns((long y, EncodingType z) => y + "_Encoded");
            mockEncodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.ApprenticeshipId)).Returns((long y, EncodingType z) => y + "_Encoded");
            row1Panel1Orchestrator = new Row1Panel1Orchestrator(mockCommitmentApiClient.Object, mockEncodingService.Object);
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
                    NumberOfDraftApprentices = 400,
                    IsDraft = false,
                    WithParty = Party.Provider,
                    CreatedOn = DateTime.Now
                },
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




        [Test]
        public async Task ThenShouldGetDraftApprenticeshipResponse()
        {
            //Act
            var result = await row1Panel1Orchestrator.GetAccount(HashedAccountId, AccountId);

            //Assert
            var expected = DraftApprenticeshipsResponse.DraftApprenticeships.Where(x => x.CourseName == "CourseName");            
            Assert.IsTrue(result.Data.CohortsCount.Equals(1));
            Assert.IsTrue(result.Data.CohortStatus.Equals(CohortStatus.WithTrainingProvider));
            Assert.IsTrue(result.Data.HasDraftApprenticeship);
            Assert.AreEqual("1_Encoded", result.Data.HashedDraftApprenticeshipId);
            Assert.AreEqual("4_Encoded", result.Data.HashedCohortReference);
            Assert.AreEqual(expected.Select(s => s.CourseName).ToList()[0], result.Data.CourseName);
            Assert.AreEqual(expected.Select(s => s.StartDate).ToList()[0], result.Data.CourseStartDate);
            //Assert.AreEqual(expected.Select(s => s.EndDate).ToList()[0], result.Data.CourseEndDate);
            Assert.IsNotNull(result);
        }


        [Test]
        public void WrireVerifyTests()
        {

        }
    }
}
