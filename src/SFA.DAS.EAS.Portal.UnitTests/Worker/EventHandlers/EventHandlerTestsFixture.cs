using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using KellermanSoftware.CompareNetObjects;
using Moq;
using NServiceBus;
using SFA.DAS.EAS.Portal.Application.Commands;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Worker.Extensions;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.EventHandlers
{
    public class EventHandlerTestsFixture<TEvent, TEventHandler, TCommand, TCommandParam>
        where TEventHandler : IHandleMessages<TEvent>
        where TCommand : class, IPortalCommand<TCommandParam>
    {
        public TEvent Message { get; set; }
        public TEvent ExpectedMessage { get; set; }
        public IHandleMessages<TEvent> Handler { get; set; }
        public string MessageId { get; set; }
        public Mock<IMessageContext> MessageContext { get; set; }
        public Mock<IMessageHandlerContext> MessageHandlerContext { get; set; }
        public Mock<TCommand> Command { get; set; }
        
        public EventHandlerTestsFixture(Func<IMessageContext, IHandleMessages<TEvent>> constructHandler = null)
        {
            var fixture = new Fixture();
            Message = fixture.Create<TEvent>();

            MessageContext = new Mock<IMessageContext>();
            
            MessageId = fixture.Create<string>();
            //todo: use nservicebus's version??
            MessageHandlerContext = new Mock<IMessageHandlerContext>();
            MessageHandlerContext.Setup(c => c.MessageId).Returns(MessageId);

            var messageHeaders = new Mock<IReadOnlyDictionary<string, string>>();
            messageHeaders.SetupGet(c => c["NServiceBus.TimeSent"]).Returns(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss:ffffff Z", CultureInfo.InvariantCulture));
            MessageHandlerContext.Setup(c => c.MessageHeaders).Returns(messageHeaders.Object);

            Command = new Mock<TCommand>();
            
            Handler = constructHandler != null ? constructHandler(MessageContext.Object) : ConstructHandler();
        }

        public virtual Task Handle()
        {
            ExpectedMessage = Message.Clone();
            return Handler.Handle(Message, MessageHandlerContext.Object);
        }

        private TEventHandler ConstructHandler()
        {
            return (TEventHandler)Activator.CreateInstance(typeof(TEventHandler), Command.Object, MessageContext.Object);
        }

        public EventHandlerTestsFixture<TEvent, TEventHandler, TCommand, TCommandParam> VerifyCommandExecutedWithUnchangedEvent()
        {
            // generic command with execute(TParam)
            // what we need to check: injected context populated correctly, command executed
            // introduce base command so can verify execute
            //return _addAccountProviderCommand.Execute(message);

            //helper for compare clone expectedmessage, alongside clone AreEqual
            Command.Verify(c => c.Execute(
                It.Is<TCommandParam>(p => new CompareLogic().Compare(ExpectedMessage,p).AreEqual), It.IsAny<CancellationToken>()),
                Times.Once);

            //todo: want test to fluent chain using derived methods, covariance?
            return this;
        }

        public EventHandlerTestsFixture<TEvent, TEventHandler, TCommand, TCommandParam> VerifyMessageContextIsInitialised()
        {
            //todo: the old chestnut of having to verify a mocked object as param. switching to nservicebus's fake should help
            //todo: extension method - change, pain to test
            //MessageContext.Verify(mc => mc.Initialise(It.IsAny<IMessageHandlerContext>()), Times.Once);

            return this;
        }
    }
}