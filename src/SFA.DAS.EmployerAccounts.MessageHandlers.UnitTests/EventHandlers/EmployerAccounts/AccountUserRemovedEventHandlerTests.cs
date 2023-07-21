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
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.UnitTests.EventHandlers.EmployerAccounts
{
    [TestFixture]
    [Parallelizable]
    internal class AccountUserRemovedEventHandlerTests : FluentTest<UserRolesRemovedEventHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingEvent_ThenShouldSendRemoveAccountUserCommand()
        {
            return TestAsync(f => f.Handler.Handle(f.Message, f.MessageHandlerContext.Object),
                f => f.Mediator.Verify(x => x.Send(It.Is<RemoveAccountUserCommand>(p =>
                        p.AccountId == f.AccountId &&
                        p.UserRef == f.UserRef &&
                        p.AccountId == f.AccountId &&
                        p.Removed == f.Created &&
                        p.MessageId == f.MessageId
                    ),
                    It.IsAny<CancellationToken>())));
        }
    }

    internal class UserRolesRemovedEventHandlerTestsFixture
    {
        public string MessageId = "messageId";
        public AccountUserRemovedEvent Message;
        public long AccountId = 333333;
        public Guid UserRef = Guid.NewGuid();

        public DateTime Created = DateTime.Now.AddMinutes(-1);

        public Mock<IMessageHandlerContext> MessageHandlerContext;
        public Mock<IMediator> Mediator;
        public AccountUserRemovedEventHandler Handler;

        public UserRolesRemovedEventHandlerTestsFixture()
        {
            Mediator = new Mock<IMediator>();
            MessageHandlerContext = new Mock<IMessageHandlerContext>();
            MessageHandlerContext.Setup(x => x.MessageId).Returns(MessageId);

            Message = new AccountUserRemovedEvent(AccountId, UserRef, Created);

            Handler = new AccountUserRemovedEventHandler(Mediator.Object);
        }
    }
}
