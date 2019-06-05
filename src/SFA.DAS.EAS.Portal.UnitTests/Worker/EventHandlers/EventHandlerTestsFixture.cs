using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
using SFA.DAS.EAS.Portal.Application.Services;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.EventHandlers
{
    public class EventHandlerTestsFixture<TEvent, TEventHandler>
        where TEventHandler : IHandleMessages<TEvent>
    {
        public TEvent Message { get; set; }
        public TEvent ExpectedMessage { get; set; }
        public IHandleMessages<TEvent> Handler { get; set; }
        public string MessageId { get; set; }
        public Mock<IMessageContextInitialisation> MessageContextInitialisation { get; set; }
        public Mock<IMessageHandlerContext> MessageHandlerContext { get; set; }
        public Mock<IAccountDocumentService> AccountDocumentService { get; set; }
        public Mock<ILogger<TEventHandler>> Logger { get; set; }
        
        public EventHandlerTestsFixture(bool constructHandler = true)
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

            AccountDocumentService = new Mock<IAccountDocumentService>();
            
            Logger = new Mock<ILogger<TEventHandler>>();
            
            if (constructHandler)
                Handler = ConstructHandler();
        }

        public virtual Task Handle()
        {
            ExpectedMessage = Message.Clone();
            return Handler.Handle(Message, MessageHandlerContext.Object);
        }

        private TEventHandler ConstructHandler()
        {
            return (TEventHandler)Activator.CreateInstance(typeof(TEventHandler), AccountDocumentService.Object, MessageContextInitialisation.Object, Logger.Object);
        }

//        public EventHandlerTestsFixture<TEvent, TEventHandler, TCommand> VerifyCommandExecutedWithUnchangedEvent()
//        {
//            Command.Verify(c => c.Execute(
//                It.Is<TEvent>(p => p.IsEqual(ExpectedMessage)), It.IsAny<CancellationToken>()),
//                Times.Once);
//
//            //todo: want test to fluent chain using derived methods, covariance?
//            return this;
//        }

        public EventHandlerTestsFixture<TEvent, TEventHandler> VerifyMessageContextIsInitialised()
        {
            MessageContextInitialisation.Verify(mc => mc.Initialise(MessageHandlerContext.Object), Times.Once);

            return this;
        }
    }
}