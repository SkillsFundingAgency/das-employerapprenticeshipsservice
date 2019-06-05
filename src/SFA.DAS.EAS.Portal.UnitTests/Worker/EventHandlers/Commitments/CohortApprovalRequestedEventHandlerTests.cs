using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Worker.EventHandlers.Commitments;
using SFA.DAS.HashingService;
using SFA.DAS.Testing;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.EventHandlers.Commitments
{
    [TestFixture]
    [Parallelizable]
    public class CohortApprovalRequestedByProviderEventHandlerTests : FluentTest<CohortApprovalRequestedByProviderEventHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingCohortApprovalRequestedByProvider_ThenShouldInitialiseMessageContext()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyMessageContextIsInitialised());
        }
    }

    public class CohortApprovalRequestedByProviderEventHandlerTestsFixture : EventHandlerTestsFixture<
        CohortApprovalRequestedByProvider, CohortApprovalRequestedByProviderEventHandler>
    {
        public Mock<IProviderCommitmentsApi> ProviderCommitmentsApi { get; set; }
        public Mock<IHashingService> HashingService { get; set; }
        public CommitmentView Commitment { get; set; }
        public Fixture Fixture { get; set; }
        public const long UnHashedId = 123L;

        public CohortApprovalRequestedByProviderEventHandlerTestsFixture() : base(false)
        {
            Fixture = new Fixture();

            Commitment = Fixture.Create<CommitmentView>();
            
            ProviderCommitmentsApi = new Mock<IProviderCommitmentsApi>();
            ProviderCommitmentsApi
                //todo: check correct providerid & commitmentid
                .Setup(m => m.GetProviderCommitment(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(Commitment);

            HashingService = new Mock<IHashingService>();
            HashingService
                    //todo: real value
                .Setup(m => m.DecodeValue(It.IsAny<string>()))
                .Returns(UnHashedId);                    
            
            Handler = new CohortApprovalRequestedByProviderEventHandler(
                AccountDocumentService.Object,
                MessageContextInitialisation.Object,
                Logger.Object,
                ProviderCommitmentsApi.Object,
                HashingService.Object);
        }
    }
}
