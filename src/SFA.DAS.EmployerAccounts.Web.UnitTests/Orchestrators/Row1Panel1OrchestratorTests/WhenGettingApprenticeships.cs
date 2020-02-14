using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.Encoding;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.Row1Panel1OrchestratorTests
{
    public class WhenGettingApprenticeships
    {
        private const string HashedAccountId = "ABC123";
        private const long AccountId = 123;
        private Mock<ICommitmentsApiClient> mockCommitmentApiClient;
        private Mock<IEncodingService> mockEncodingService;
        private GetApprenticeshipsResponse ApprenticeshipsResponse;
        private Row1Panel1Orchestrator row1Panel1Orchestrator;


        [SetUp]
        public void Arrange()
        {   
            //Arrange
            mockCommitmentApiClient = new Mock<ICommitmentsApiClient>();
            mockEncodingService = new Mock<IEncodingService>();
            ApprenticeshipsResponse = CreateApprenticeshipResponse();

            mockCommitmentApiClient.Setup(c => c.GetApprenticeships(It.Is<GetApprenticeshipsRequest>(a => a.AccountId == AccountId), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(ApprenticeshipsResponse));
            mockEncodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns((long y, EncodingType z) => y + "_Encoded");
            mockEncodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.ApprenticeshipId)).Returns((long y, EncodingType z) => y + "_Encoded");

            row1Panel1Orchestrator = new Row1Panel1Orchestrator(mockCommitmentApiClient.Object, mockEncodingService.Object);
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
                    ApprenticeshipStatus = ApprenticeshipStatus.Live
                }
            };

            return new GetApprenticeshipsResponse() { Apprenticeships = apprenticeships, TotalApprenticeships = 1, TotalApprenticeshipsFound = 1, TotalApprenticeshipsWithAlertsFound =0 };
        }

         [Test]
        public async Task ThenShouldGetApprenticeshipResponse()
        {
            //Act
            var result = await row1Panel1Orchestrator.GetAccount(HashedAccountId, AccountId);

            //Assert            
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.ApprenticeshipsCount.Equals(1));
        }        
    }
}
