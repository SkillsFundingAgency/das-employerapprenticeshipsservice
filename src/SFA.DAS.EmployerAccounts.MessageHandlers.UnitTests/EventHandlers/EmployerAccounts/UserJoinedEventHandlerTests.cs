using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers.EmployerAccounts;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;
using SFA.DAS.EmployerAccounts.Types.Models;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.UnitTests.EventHandlers.EmployerAccounts
{
    [TestFixture]
    [Parallelizable]
    internal class UserJoinedEventHandlerTests : FluentTest<UserJoinedEventHandlerForReadStoreTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingEvent_ThenShouldSendCreateAccountUserCommand()
        {
            return TestAsync(f => f.Handler.Handle(f.Message, f.MessageHandlerContext.Object),
                f => f.Mediator.Verify(x => x.Send(It.Is<CreateAccountUserCommand>(p =>
                        p.AccountId == f.AccountId &&
                        p.UserRef == f.UserRef &&
                        p.Role == f.Role &&
                        p.Created == f.Created &&
                        p.MessageId == f.MessageId
                    ),
                    It.IsAny<CancellationToken>())));
        }
    }

    internal class UserJoinedEventHandlerForReadStoreTestsFixture
    {
        public string MessageId = "messageId";
        public UserJoinedEvent Message;
        public long AccountId = 333333;
        public Guid UserRef = Guid.NewGuid();
        public long UserId = 877664;

        public UserRole Role = UserRole.Transactor;
        public DateTime Created = DateTime.Now.AddMinutes(-1);

        public Mock<IMessageHandlerContext> MessageHandlerContext;
        public Mock<IMediator> Mediator;
        public UserJoinedEventHandler Handler;

        public UserJoinedEventHandlerForReadStoreTestsFixture()
        {
            Mediator = new Mock<IMediator>();
            MessageHandlerContext = new Mock<IMessageHandlerContext>();
            MessageHandlerContext.Setup(x => x.MessageId).Returns(MessageId);

            Message = new UserJoinedEvent {
                AccountId = AccountId,
                UserRef = UserRef,
                Role = Role,
                Created = Created};

            Handler = new UserJoinedEventHandler(Mediator.Object);
        }
    }
}
