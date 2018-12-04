using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.EmployerAccounts.Types.Models;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.UnitTests.EventHandlers
{
    [TestFixture]
    [Parallelizable]
    internal class UserJoinedEventHandlerForReadStoreTests : FluentTest<UserJoinedEventHandlerForReadStoreTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingEvent_ThenShouldSendCreateAccountUserCommand()
        {
            return TestAsync(f => f.Handler.Handle(f.Message, f.MessageHandlerContext.Object),
                f => f.ReadStoreMediator.Verify(x => x.Send(It.Is<CreateAccountUserCommand>(p =>
                        p.AccountId == f.AccountId &&
                        p.UserRef == f.UserRef &&
                        p.Roles == f.Roles &&
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

        public HashSet<UserRole> Roles = new HashSet<UserRole>();
        public DateTime Created = DateTime.Now.AddMinutes(-1);

        public Mock<IMessageHandlerContext> MessageHandlerContext;
        public Mock<IReadStoreMediator> ReadStoreMediator;
        public UserJoinedEventHandlerForReadStore Handler;

        public UserJoinedEventHandlerForReadStoreTestsFixture()
        {
            ReadStoreMediator = new Mock<IReadStoreMediator>();
            MessageHandlerContext = new Mock<IMessageHandlerContext>();
            MessageHandlerContext.Setup(x => x.MessageId).Returns(MessageId);

            Message = new UserJoinedEvent {
                AccountId = AccountId,
                UserRef = UserRef,
                Roles = Roles,
                Created = Created};

            Handler = new UserJoinedEventHandlerForReadStore(ReadStoreMediator.Object);
        }
    }
}
