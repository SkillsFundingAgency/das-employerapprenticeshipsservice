using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Application.EventHandlers;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.Testing;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.EventHandlers
{
    [TestFixture, Parallelizable]
    public class EventHandlerTests : FluentTest<EventHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingEvent_ThenShouldInitialiseMessageContext()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyMessageContextIsInitialised());
        }
        
        [Test]
        public Task Handle_WhenHandlingEvent_ThenShouldDelegateHandlingToHandlerPartTwo()
        {
            return TestAsync(f => f.Handle(), f => f.VerifyCommandExecutedWithUnchangedEvent());
        }
    }

    public class EventHandlerTestsFixture
    {
        public object Message { get; set; }
        public object ExpectedMessage { get; set; }
        public IHandleMessages<object> Handler { get; set; }
        public string MessageId { get; set; }
        public Mock<IMessageContextInitialisation> MessageContextInitialisation { get; set; }
        public Mock<IMessageHandlerContext> MessageHandlerContext { get; set; }
        public Mock<IEventHandler<object>> EventHandlerPartTwo { get; set; }
        public Mock<ILogger> Logger { get; set; }

        public EventHandlerTestsFixture()
        {
            var fixture = new Fixture();
            Message = new object();

            MessageContextInitialisation = new Mock<IMessageContextInitialisation>();
            
            MessageId = fixture.Create<string>();
            MessageHandlerContext = new Mock<IMessageHandlerContext>();
            MessageHandlerContext.Setup(c => c.MessageId).Returns(MessageId);

            var messageHeaders = new Mock<IReadOnlyDictionary<string, string>>();
            messageHeaders.SetupGet(c => c["NServiceBus.TimeSent"]).Returns(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss:ffffff Z", CultureInfo.InvariantCulture));
            MessageHandlerContext.Setup(c => c.MessageHeaders).Returns(messageHeaders.Object);

            EventHandlerPartTwo = new Mock<IEventHandler<object>>();
                
            Logger = new Mock<ILogger>();
            
            Handler = (IHandleMessages<object>)Activator.CreateInstance(
                typeof(SFA.DAS.EAS.Portal.Worker.EventHandlers.EventHandler<object>),
                BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null,
                new object[] {MessageContextInitialisation.Object, EventHandlerPartTwo.Object, Logger.Object},
                null, null);
        }

        public Task Handle()
        {
            ExpectedMessage = Message.Clone();
            return Handler.Handle(Message, MessageHandlerContext.Object);
        }

        public void VerifyCommandExecutedWithUnchangedEvent()
        {
            EventHandlerPartTwo.Verify(eh => eh.Handle(
                    //todo: need to check correct object is passed, create dummy message?
                    It.IsAny<object>(), //e => e.IsEqual(ExpectedMessage).Item1),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        public void VerifyMessageContextIsInitialised()
        {
            MessageContextInitialisation.Verify(mc => mc.Initialise(MessageHandlerContext.Object), Times.Once);
        }
    }
}