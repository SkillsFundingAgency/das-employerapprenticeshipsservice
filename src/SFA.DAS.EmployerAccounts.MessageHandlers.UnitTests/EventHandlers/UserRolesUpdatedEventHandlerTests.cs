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
    internal class UserRolesUpdatedEventHandlerTests : FluentTest<UserRolesUpdatedEventHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingEvent_ThenShouldSendUpdateUserRolesCommand()
        {
            return TestAsync(f => f.Handler.Handle(f.Message, f.MessageHandlerContext.Object),
                f => f.ReadStoreMediator.Verify(x => x.Send(It.Is<UpdateAccountUserCommand>(p =>
                        p.AccountId == f.AccountId &&
                        p.UserRef == f.UserRef &&
                        p.UserId == f.UserId &&
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
        public UserRolesUpdatedEvent Message;
        public long AccountId = 333333;
        public Guid UserRef = Guid.NewGuid();
        public long UserId = 877664;

        public HashSet<UserRole> Roles = new HashSet<UserRole>();
        public DateTime Created = DateTime.Now.AddMinutes(-1);

        public Mock<IMessageHandlerContext> MessageHandlerContext;
        public Mock<IReadStoreMediator> ReadStoreMediator;
        public UserRolesUpdatedEventHandler Handler;

        public UserRolesUpdatedEventHandlerTestsFixture()
        {
            ReadStoreMediator = new Mock<IReadStoreMediator>();
            MessageHandlerContext = new Mock<IMessageHandlerContext>();
            MessageHandlerContext.Setup(x => x.MessageId).Returns(MessageId);

            Message = new UserRolesUpdatedEvent(AccountId, UserRef, UserId, Roles, Created);

            Handler = new UserRolesUpdatedEventHandler(ReadStoreMediator.Object);
        }
    }
}
