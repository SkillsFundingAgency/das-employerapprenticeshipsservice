using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.EventHandlers;
using SFA.DAS.EAS.Portal.Worker.EventHandlers.Commitments;
using SFA.DAS.Testing;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.EventHandlers.Commitments
{
    [TestFixture, Parallelizable]
    public class CohortApprovalRequestedEventHandlerTests : FluentTest<CohortApprovalRequestedEventHandlerFixture>
    {
        [Test]
        public Task Handle_WhenHandlingCohortApprovalRequestedByProvider_ThenShouldInitialiseMessageContext()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyMessageContextIsInitialised());
        }

        [Test]
        public Task Handle_WhenHandlingCohortApprovalRequestedByProvider_ThenShouldPassTheEventToTheDomainHandler()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyDomainHandlerCalled());
        }
    }

    public class CohortApprovalRequestedEventHandlerFixture : EventHandlerTestsFixture<CohortApprovalRequestedByProvider, CohortApprovalRequestedByProviderEventHandler>
    {
        public Mock<IEventHandler<CohortApprovalRequestedByProvider>> MockHandler { get; set; }

        public CohortApprovalRequestedEventHandlerFixture()
        {
            MockHandler = new Mock<IEventHandler<CohortApprovalRequestedByProvider>>();

            Handler = new CohortApprovalRequestedByProviderEventHandler(
                MockHandler.Object,
                MockMessageContextInitialisation.Object);
        }

        public void VerifyDomainHandlerCalled()
        {
            MockHandler.Verify(s => s.Handle(Message, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
