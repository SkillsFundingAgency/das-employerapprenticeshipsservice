using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.UnitTests.EventHandlers
{
    [TestFixture]
    [Parallelizable]
    internal class AccountUserRemovedEventHandlerTests : FluentTest<UserRolesRemovedEventHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingEvent_ThenShouldSendRemoveAccountUserCommand()
        {
            return TestAsync(f => f.Handler.Handle(f.Message, f.MessageHandlerContext.Object),
                f => f.ReadStoreMediator.Verify(x => x.Send(It.Is<RemoveAccountUserCommand>(p =>
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
        public AccountUserRolesRemovedEvent Message;
        public long AccountId = 333333;
        public Guid UserRef = Guid.NewGuid();

        public DateTime Created = DateTime.Now.AddMinutes(-1);

        public Mock<IMessageHandlerContext> MessageHandlerContext;
        public Mock<IReadStoreMediator> ReadStoreMediator;
        public AccountUserRemovedEventHandler Handler;

        public UserRolesRemovedEventHandlerTestsFixture()
        {
            ReadStoreMediator = new Mock<IReadStoreMediator>();
            MessageHandlerContext = new Mock<IMessageHandlerContext>();
            MessageHandlerContext.Setup(x => x.MessageId).Returns(MessageId);

            Message = new AccountUserRolesRemovedEvent(AccountId, UserRef, Created);

            Handler = new AccountUserRemovedEventHandler(ReadStoreMediator.Object);
        }
    }
}
