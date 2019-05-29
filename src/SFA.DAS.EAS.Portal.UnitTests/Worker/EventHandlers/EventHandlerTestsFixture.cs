using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NServiceBus;
using SFA.DAS.EAS.Portal.Application.Services;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.EventHandlers
{
    public class EventHandlerTestsFixture<TEvent, TEventHandler, TICommand>
        where TEventHandler : IHandleMessages<TEvent>
        where TICommand : class
    {
        public TEvent Message { get; set; }
        public IHandleMessages<TEvent> Handler { get; set; }
        public string MessageId { get; set; }
        public Mock<IMessageContext> MessageContext { get; set; }
        public Mock<IMessageHandlerContext> MessageHandlerContext { get; set; }
        public Mock<TICommand> Command { get; set; }
        
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

            Command = new Mock<TICommand>();
            
            Handler = constructHandler != null ? constructHandler(MessageContext.Object) : ConstructHandler();
        }

//        protected abstract TCommand CreateCommand();
        
        public virtual Task Handle()
        {
            return Handler.Handle(Message, MessageHandlerContext.Object);
        }

        private TEventHandler ConstructHandler()
        {
//            return (TEventHandler)Activator.CreateInstance(typeof(TEventHandler), CreateCommand(), MessageContext.Object);
            return (TEventHandler)Activator.CreateInstance(typeof(TEventHandler), Command.Object, MessageContext.Object);
        }

        public void VerifyCommandExecuted()
        {
            // introduce base command so can verify execute
            //return _addAccountProviderCommand.Execute(message);

        }
    }
}