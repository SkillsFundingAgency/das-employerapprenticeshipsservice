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
    internal class UserRolesRemovedEventHandlerTests : FluentTest<UserRolesRemovedEventHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingEvent_ThenShouldSendRemoveUserRolesCommand()
        {
            return TestAsync(f => f.Handler.Handle(f.Message, f.MessageHandlerContext.Object),
                f => f.ReadStoreMediator.Verify(x => x.Send(It.Is<RemoveAccountUserCommand>(p =>
                        p.AccountId == f.AccountId &&
                        p.UserId == f.UserId &&
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
        public UserRolesRemovedEvent Message;
        public long AccountId = 333333;
        public long UserId = 877664;

        public DateTime Created = DateTime.Now.AddMinutes(-1);

        public Mock<IMessageHandlerContext> MessageHandlerContext;
        public Mock<IReadStoreMediator> ReadStoreMediator;
        public UserRolesRemovedEventHandler Handler;

        public UserRolesRemovedEventHandlerTestsFixture()
        {
            ReadStoreMediator = new Mock<IReadStoreMediator>();
            MessageHandlerContext = new Mock<IMessageHandlerContext>();
            MessageHandlerContext.Setup(x => x.MessageId).Returns(MessageId);

            Message = new UserRolesRemovedEvent(AccountId, UserId, Created);

            Handler = new UserRolesRemovedEventHandler(ReadStoreMediator.Object);
        }
    }
}
