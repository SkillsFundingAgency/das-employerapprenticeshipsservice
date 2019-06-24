using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Worker.EventHandlers.Commitments;
using SFA.DAS.Testing;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.EventHandlers;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.EventHandlers.Commitments
{
    [TestFixture]
    [Parallelizable]
    public class CohortApprovedByEmployerEventHandlerTests : FluentTest<CohortApprovedByEmployerEventHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingCohortApprovedByEmployer_ThenShouldInitialiseMessageContext()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyMessageContextIsInitialised());
        }

        [Test]
        public Task Handle_WhenHandlingCohortApprovedByEmployer_ThenShouldPassTheEventToTheDomainHandler()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyMessageContextIsInitialised());
        }
                
    }

    public class CohortApprovedByEmployerEventHandlerTestsFixture : EventHandlerTestsFixture<CohortApprovedByEmployer, CohortApprovedByEmployerEventHandler>
    {
        public Mock<IEventHandler<CohortApprovedByEmployer>> HandlerMock { get; set; }

        public CohortApprovedByEmployerEventHandlerTestsFixture()
        {
            HandlerMock = new Mock<IEventHandler<CohortApprovedByEmployer>>();

            Handler = new CohortApprovedByEmployerEventHandler(
                HandlerMock.Object,
                MessageContextInitialisationMock.Object);
        }

        public void VerifyDomainHandlerCalled()
        {
            HandlerMock.Verify(s => s.Handle(Message, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
