using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NServiceBus;
using SFA.DAS.EAS.Portal.Application.Commands;
using SFA.DAS.EAS.Portal.Application.Services;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.EventHandlers
{
    public class EventHandlerTestsFixture<TEvent, TEventHandler, TCommand>
        where TEventHandler : IHandleMessages<TEvent>
        where TCommand : class, ICommand<TEvent>
    {
        public TEvent Message { get; set; }
        public TEvent ExpectedMessage { get; set; }
        public IHandleMessages<TEvent> Handler { get; set; }
        public string MessageId { get; set; }
        public Mock<IMessageContextInitialisation> MessageContextInitialisation { get; set; }
        public Mock<IMessageHandlerContext> MessageHandlerContext { get; set; }
        public Mock<TCommand> Command { get; set; }
        
        public EventHandlerTestsFixture(Func<IMessageContextInitialisation, IHandleMessages<TEvent>> constructHandler = null)
        {
            var fixture = new Fixture();
            Message = fixture.Create<TEvent>();

            MessageContextInitialisation = new Mock<IMessageContextInitialisation>();
            
            MessageId = fixture.Create<string>();
            // nservicebus's TestableMessageHandlerContext is available, but we don't need it yet
            MessageHandlerContext = new Mock<IMessageHandlerContext>();
            MessageHandlerContext.Setup(c => c.MessageId).Returns(MessageId);

            var messageHeaders = new Mock<IReadOnlyDictionary<string, string>>();
            messageHeaders.SetupGet(c => c["NServiceBus.TimeSent"]).Returns(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss:ffffff Z", CultureInfo.InvariantCulture));
            MessageHandlerContext.Setup(c => c.MessageHeaders).Returns(messageHeaders.Object);

            Command = new Mock<TCommand>();
            
            Handler = constructHandler != null ? constructHandler(MessageContextInitialisation.Object) : ConstructHandler();
        }

        public virtual Task Handle()
        {
            ExpectedMessage = Message.Clone();
            return Handler.Handle(Message, MessageHandlerContext.Object);
        }

        private TEventHandler ConstructHandler()
        {
            return (TEventHandler)Activator.CreateInstance(typeof(TEventHandler), Command.Object, MessageContextInitialisation.Object);
        }

        public EventHandlerTestsFixture<TEvent, TEventHandler, TCommand> VerifyCommandExecutedWithUnchangedEvent()
        {
            Command.Verify(c => c.Execute(
                It.Is<TEvent>(p => p.IsEqual(ExpectedMessage)), It.IsAny<CancellationToken>()),
                Times.Once);

            //todo: want test to fluent chain using derived methods, covariance?
            return this;
        }

        public EventHandlerTestsFixture<TEvent, TEventHandler, TCommand> VerifyMessageContextIsInitialised()
        {
            MessageContextInitialisation.Verify(mc => mc.Initialise(MessageHandlerContext.Object), Times.Once);

            return this;
        }
    }
}