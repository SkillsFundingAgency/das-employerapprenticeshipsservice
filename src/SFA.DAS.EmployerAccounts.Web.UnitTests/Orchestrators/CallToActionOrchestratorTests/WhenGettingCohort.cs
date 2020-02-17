using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.EmployerAccounts.Queries.GetReservations;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.Encoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.CallToActionOrchestratorTests
{
    public class WhenGettingCohort
    {
        private const string HashedAccountId = "ABC123";
        private const long AccountId = 123;
        public const long ProviderId = 789;
        private const string UserId = "USER1";        
        private CallToActionOrchestrator callToActionOrchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ICommitmentsApiClient> mockCommitmentApiClient;
        private Mock<IEncodingService> mockEncodingService;
        public GetCohortsResponse GetCohortsResponse { get; set; }

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

            GetCohortsResponse = CreateGetCohortsResponseForDraftStaus();
            mockCommitmentApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.AccountId == AccountId), CancellationToken.None)).Returns(Task.FromResult(GetCohortsResponse));
            mockEncodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns((long y, EncodingType z) => y + "_Encoded");
            mockEncodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.ApprenticeshipId)).Returns((long y, EncodingType z) => y + "_Encoded");
            callToActionOrchestrator = new CallToActionOrchestrator(_mediator.Object, mockCommitmentApiClient.Object, mockEncodingService.Object);
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
                    NumberOfDraftApprentices = 0,
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
            var result = await callToActionOrchestrator.GetCallToAction(HashedAccountId, AccountId, UserId);

            //Assert
            var expected = GetCohortsResponse.Cohorts.Where(x => x.WithParty == Party.Employer);            
            Assert.IsTrue(result.Data.CohortsCount.Equals(1));
            Assert.IsTrue(result.Data.CohortStatus.Equals(CohortStatus.Draft));
            Assert.AreEqual(expected.Select(x => x.NumberOfDraftApprentices).ToList()[0], result.Data.NumberOfDraftApprentices);
            Assert.AreEqual(expected.Select(x => x.ProviderName).ToList()[0], result.Data.ProviderName);
        }

    }
}
