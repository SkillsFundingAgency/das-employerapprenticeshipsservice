using AutoFixture;
using SFA.DAS.EAS.Portal.Application.EventHandlers;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.UnitTests.Portal.Application
{
    public class EventHandlerTestsFixture<TEvent, TEventHandler>
        where TEventHandler : IEventHandler<TEvent>
    {
        public TEvent Event { get; set; }
        public TEvent OriginalEvent { get; set; }
        public IEventHandler<TEvent> Handler { get; set; }
        public Fixture Fixture { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public EventHandlerTestsFixture()
        {
            Fixture = new Fixture();

            Event = Fixture.Create<TEvent>();
            CancellationToken = new CancellationToken();
        }

        public virtual Task Handle()
        {
            OriginalEvent = Event.Clone();
            return Handler.Handle(Event, CancellationToken);
        }
    }
}
