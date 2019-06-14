using AutoFixture;
using Moq;
using NServiceBus;
using SFA.DAS.EAS.Portal.Application.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.EventHandlers
{
    public class EventHandlerTestsFixture<TEvent, TEventHandler>
        where TEventHandler : IHandleMessages<TEvent>
    {
        public TEvent Message { get; set; }
        public TEvent OriginalMessage { get; set; }
        public IHandleMessages<TEvent> Handler { get; set; }
        public string MessageId { get; set; }
        public Mock<IMessageContextInitialisation> MockMessageContextInitialisation { get; set; }
        public Mock<IMessageHandlerContext> MessageHandlerContext { get; set; }
        public Fixture Fixture { get; set; }

        public EventHandlerTestsFixture()
        {
            Fixture = new Fixture();

            Message = Fixture.Create<TEvent>();

            MockMessageContextInitialisation = new Mock<IMessageContextInitialisation>();

            MessageId = Fixture.Create<string>();
            MessageHandlerContext = new Mock<IMessageHandlerContext>();
            MessageHandlerContext.Setup(c => c.MessageId).Returns(MessageId);

            var messageHeaders = new Mock<IReadOnlyDictionary<string, string>>();
            messageHeaders.SetupGet(c => c["NServiceBus.TimeSent"]).Returns(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss:ffffff Z", CultureInfo.InvariantCulture));
            MessageHandlerContext.Setup(c => c.MessageHeaders).Returns(messageHeaders.Object);
        }

        public virtual Task Handle()
        {
            OriginalMessage = Message.Clone();
            return Handler.Handle(Message, MessageHandlerContext.Object);
        }

        public void VerifyMessageContextIsInitialised()
        {
            MockMessageContextInitialisation.Verify(mc => mc.Initialise(MessageHandlerContext.Object), Times.Once);
        }
    }
}
