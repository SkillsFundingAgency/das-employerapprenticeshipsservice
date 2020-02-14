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

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.Row1Panel1OrchestratorTests
{
    public class WhenGettingCohort
    {
        private const string HashedAccountId = "ABC123";
        private const long AccountId = 123;
        public const long ProviderId = 789;
        private Row1Panel1Orchestrator row1Panel1Orchestrator;
        private Mock<ICommitmentsApiClient> mockCommitmentApiClient;
        private Mock<IEncodingService> mockEncodingService;
        public GetCohortsResponse GetCohortsResponse { get; set; }

        [SetUp]
        public void Arrange()
        {
            //Arrange
            mockCommitmentApiClient = new Mock<ICommitmentsApiClient>();
            mockEncodingService = new Mock<IEncodingService>();
            
            GetCohortsResponse = CreateGetCohortsResponseForDraftStaus();
            mockCommitmentApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.AccountId == AccountId), CancellationToken.None)).Returns(Task.FromResult(GetCohortsResponse));
            mockEncodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns((long y, EncodingType z) => y + "_Encoded");
            mockEncodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.ApprenticeshipId)).Returns((long y, EncodingType z) => y + "_Encoded");
            row1Panel1Orchestrator = new Row1Panel1Orchestrator(mockCommitmentApiClient.Object, mockEncodingService.Object);
        }

        private GetCohortsResponse CreateGetCohortsResponseForDraftStaus()
        {
            IEnumerable<CohortSummary> cohorts = new List<CohortSummary>()
            {
                 new CohortSummary
                {
                    CohortId = 2,
                    AccountId = 1,
                    ProviderId = 2,
                    ProviderName = "Provider2",
                    NumberOfDraftApprentices = 1,
                    IsDraft = true,
                    WithParty = Party.Employer,
                    CreatedOn = DateTime.Now.AddMinutes(-5),
                }
            };

            return new GetCohortsResponse(cohorts);
        }


        [Test]
        public async Task ThenShouldGetCohortResponse()
        {
            //Act
            var result = await row1Panel1Orchestrator.GetAccount(HashedAccountId, AccountId);

            //Assert
            var expected = GetCohortsResponse.Cohorts.Where(x => x.WithParty == Party.Employer);            
            Assert.IsTrue(result.Data.CohortsCount.Equals(1));
            Assert.IsTrue(result.Data.CohortStatus.Equals(CohortStatus.Draft));
            Assert.AreEqual(expected.Select(x => x.NumberOfDraftApprentices).ToList()[0], result.Data.NumberOfDraftApprentices);
            Assert.AreEqual(expected.Select(x => x.ProviderName).ToList()[0], result.Data.ProviderName);
        }

    }
}
