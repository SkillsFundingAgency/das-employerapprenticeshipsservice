using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Application.Commands;
using SFA.DAS.EAS.Portal.Worker.EventHandlers.Reservations;
using SFA.DAS.Reservations.Messages;
using SFA.DAS.Testing;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.EventHandlers.Reservations
{
    [TestFixture]
    [Parallelizable]
    public class ReservationCreatedEventHandlerTests : FluentTest<ReservationCreatedEventHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingReservationCreatedEvent_ThenShouldInitialiseMessageContext()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyMessageContextIsInitialised());
        }
        
        [Test]
        public Task Handle_WhenHandlingReservationCreatedEvent_ThenShouldExecuteAddReservationCommand()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyCommandExecutedWithUnchangedEvent());
        }
    }

    public class ReservationCreatedEventHandlerTestsFixture : EventHandlerTestsFixture<
        ReservationCreatedEvent, ReservationCreatedEventHandler, IPortalCommand<ReservationCreatedEvent>, ReservationCreatedEvent>
    {
    }
}