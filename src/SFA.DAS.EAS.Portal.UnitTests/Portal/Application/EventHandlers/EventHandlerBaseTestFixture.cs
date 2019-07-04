using System;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.EAS.Portal.Application.EventHandlers;

namespace SFA.DAS.EAS.Portal.UnitTests.Portal.Application.EventHandlers
{
    public class EventHandlerBaseTestFixture<TEvent, TEventHandler> where TEventHandler : IEventHandler<TEvent>
    {
        public Fixture Fixture { get; set; }
        public TEvent Message { get; set; }
        public TEvent OriginalMessage { get; set; }
        public IEventHandler<TEvent> Handler { get; set; }
        public Mock<ILogger<TEventHandler>> Logger { get; set; }

        public EventHandlerBaseTestFixture()
        {
            Fixture = new Fixture();
            Message = Fixture.Create<TEvent>();
            Logger = new Mock<ILogger<TEventHandler>>();
        }

        public virtual Task Handle()
        {
            OriginalMessage = Message.Clone();
            return Handler.Handle(Message);
        }
    }
}