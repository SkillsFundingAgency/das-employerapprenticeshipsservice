using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Application.EventHandlers;
using SFA.DAS.EAS.Portal.Worker.EventHandlers.Reservations;
using SFA.DAS.Reservations.Messages;
using SFA.DAS.Testing;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.EventHandlers.Reservations
{
    [TestFixture, Parallelizable]
    public class ReservationCreatedEventHandlerTests : FluentTest<ReservationCreatedEventHandlerFixture>
    {
        [Test]
        public Task Handle_WhenHandlingAddedAccountProviderEvent_ThenShouldInitialiseMessageContext()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyMessageContextIsInitialised());
        }

        [Test]
        public Task Handle_WhenHandlingAddedAccountProviderEvent_ThenShouldPassTheEventToTheDomainHandler()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyDomainHandlerCalled());
        }
    }

    public class ReservationCreatedEventHandlerFixture : EventHandlerTestsFixture<ReservationCreatedEvent, ReservationCreatedEventHandler>
    {
        public Mock<IEventHandler<ReservationCreatedEvent>> HandlerMock { get; set; }

        public ReservationCreatedEventHandlerFixture()
        {
            HandlerMock = new Mock<IEventHandler<ReservationCreatedEvent>>();

            Handler = new ReservationCreatedEventHandler(
                HandlerMock.Object,
                MessageContextInitialisationMock.Object);
        }

        public void VerifyDomainHandlerCalled()
        {
            HandlerMock.Verify(s => s.Handle(Message, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
