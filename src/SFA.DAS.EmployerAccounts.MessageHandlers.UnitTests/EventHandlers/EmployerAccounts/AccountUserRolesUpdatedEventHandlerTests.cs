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
    internal class AccountUserRolesUpdatedEventHandlerTests : FluentTest<UserRolesUpdatedEventHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingEvent_ThenShouldSendUpdateAccountUserCommand()
        {
            return TestAsync(f => f.Handler.Handle(f.Message, f.MessageHandlerContext.Object),
                f => f.ReadStoreMediator.Verify(x => x.Send(It.Is<UpdateAccountUserCommand>(p =>
                        p.AccountId == f.AccountId &&
                        p.UserRef == f.UserRef &&
                        p.AccountId == f.AccountId &&
                        p.Updated == f.Created &&
                        p.MessageId == f.MessageId
                    ),
                    It.IsAny<CancellationToken>())));
        }
    }

    internal class UserRolesUpdatedEventHandlerTestsFixture
    {
        public string MessageId = "messageId";
        public AccountUserRolesUpdatedEvent Message;
        public long AccountId = 333333;
        public Guid UserRef = Guid.NewGuid();
        public long UserId = 877664;

        public UserRole Role = UserRole.Transactor;
        public DateTime Created = DateTime.Now.AddMinutes(-1);

        public Mock<IMessageHandlerContext> MessageHandlerContext;
        public Mock<IMediator> ReadStoreMediator;
        public AccountUserRolesUpdatedEventHandler Handler;

        public UserRolesUpdatedEventHandlerTestsFixture()
        {
            ReadStoreMediator = new Mock<IMediator>();
            MessageHandlerContext = new Mock<IMessageHandlerContext>();
            MessageHandlerContext.Setup(x => x.MessageId).Returns(MessageId);

            Message = new AccountUserRolesUpdatedEvent(AccountId, UserRef, Role, Created);

            Handler = new AccountUserRolesUpdatedEventHandler(ReadStoreMediator.Object);
        }
    }
}
