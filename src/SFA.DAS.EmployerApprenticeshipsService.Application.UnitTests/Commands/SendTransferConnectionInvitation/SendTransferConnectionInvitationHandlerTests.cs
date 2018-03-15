using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.SendTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.TestCommon.Builders;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.SendTransferConnectionInvitation
{
    [TestFixture]
    public class SendTransferConnectionInvitationHandlerTests
    {
        [Test]
        public Task Handle_SuccessfulCall_ShouldShouldGetUser()
        {
            return CheckHandler(fixtures => fixtures.UserRepositoryMock.Verify(r => r.GetUserById(fixtures.SenderUser.Id), Times.Once));
        }

        [Test]
        public Task Handle_SuccessfulCall_ShouldGetSendersAccount()
        {
            return CheckHandler(fixtures => fixtures.EmployerAccountRepositoryMock.Verify(r => r.GetAccountById(fixtures.ReceiverAccount.Id), Times.Once));
        }

        [Test]
        public Task Handle_SuccessfulCall_ShouldGetReceiversAccount()
        {
            return CheckHandler(fixtures => fixtures.EmployerAccountRepositoryMock.Verify(r => r.GetAccountById(fixtures.ReceiverAccount.Id), Times.Once));
        }

        [Test]
        public Task Handle_SuccessfulCall_ShouldAddTransferConnectionInvitationToRepository()
        {
            return CheckHandler(fixtures => fixtures.TransferConnectionInvitationRepositoryMock.Verify(r => r.Add(It.IsAny<TransferConnectionInvitation>()), Times.Once));
        }

        [Test]
        public Task Handle_SuccessfulCall_ShouldReturnTransferConnectionInvitationId()
        {
            return CheckHandler(fixtures => Assert.That(fixtures.Result, Is.Not.Null));
        }

        [TestCase(TransferConnectionInvitationStatus.Pending)]
        [TestCase(TransferConnectionInvitationStatus.Approved)]
        public void ThenShouldThrowExceptionIfTransferConnectionInvitationAlreadyExists(TransferConnectionInvitationStatus invitationStatus)
        {
            Assert.ThrowsAsync<Exception>(
                () => CheckHandler(setup: fixtures => fixtures.AddInvitationFromSenderToReceiver(invitationStatus)), 
                "Requires transfer connection invitation does not already exist.");
        }

        [Test]
        public Task ThenShouldNotThrowExceptionIfRejectedTransferConnectionInvitationAlreadyExists()
        {
            return CheckHandler(setup: fixtures => fixtures.AddInvitationFromSenderToReceiver(TransferConnectionInvitationStatus.Rejected));
        }

        [Test]
        public Task Handle_PublishSentTransferConnectionInvitationEvent_ShouldNotBeNull()
        {
            return CheckSentConnectionEvent((fixtures, message) =>
                Assert.That(message, Is.Not.Null));
        }

        [Test]
        public Task Handle_PublishSentTransferConnectionInvitationEvent_ShouldHaveCreatedDateSet()
        {
            return CheckSentConnectionEvent((fixtures, message) =>
                Assert.That(message.CreatedAt,
                    Is.EqualTo(fixtures.TransferConnectionInvitation.Changes.Select(c => c.CreatedDate).Cast<DateTime?>()
                        .SingleOrDefault())));
        }

        [Test]
        public Task Handle_PublishSentTransferConnectionInvitationEvent_ShouldHaveReceiverAccountHashedIdSet()
        {
            return CheckSentConnectionEvent((fixtures, message) =>
                Assert.That(message.ReceiverAccountHashedId, Is.EqualTo(fixtures.ReceiverAccount.HashedId)));
        }

        [Test]
        public Task Handle_PublishSentTransferConnectionInvitationEvent_ShouldHaveReceiverAccountIdSet()
        {
            return CheckSentConnectionEvent((fixtures, message) => Assert.AreEqual(fixtures.ReceiverAccount.Id, message.ReceiverAccountId));
        }

        [Test]
        public Task Handle_PublishSentTransferConnectionInvitationEvent_ShouldHaveReceiverAccountNameSet()
        {
            return CheckSentConnectionEvent((fixtures, message) => Assert.AreEqual(fixtures.ReceiverAccount.Name, message.ReceiverAccountName));
        }

        [Test]
        public Task Handle_PublishSentTransferConnectionInvitationEvent_ShouldHaveSenderAccountHashedIdSet()
        {
            return CheckSentConnectionEvent((fixtures, message) => Assert.AreEqual(fixtures.SenderAccount.HashedId, message.SenderAccountHashedId));
        }

        [Test]
        public Task Handle_PublishSentTransferConnectionInvitationEvent_ShouldHaveSenderAccountIdSet()
        {
            return CheckSentConnectionEvent((fixtures, message) => Assert.AreEqual(fixtures.SenderAccount.Id, message.SenderAccountId));
        }

        [Test]
        public Task Handle_PublishSentTransferConnectionInvitationEvent_ShouldHaveSenderAccountNameSet()
        {
            return CheckSentConnectionEvent((fixtures, message) => Assert.AreEqual(fixtures.SenderAccount.Name, message.SenderAccountName));
        }

        [Test]
        public Task Handle_PublishSentTransferConnectionInvitationEvent_ShouldHaveExternalIdSet()
        {
            return CheckSentConnectionEvent((fixtures, message) => Assert.AreEqual(fixtures.SenderUser.ExternalId, message.SentByUserExternalId));
        }

        [Test]
        public Task Handle_PublishSentTransferConnectionInvitationEvent_ShouldHaveSenderUserIdSet()
        {
            return CheckSentConnectionEvent((fixtures, message) => Assert.AreEqual(fixtures.SenderUser.Id, message.SentByUserId));
        }

        [Test]
        public Task Handle_PublishSentTransferConnectionInvitationEvent_ShouldHaveSenderFullNameSet()
        {
            return CheckSentConnectionEvent((fixtures, message) => Assert.AreEqual(fixtures.SenderUser.FullName, message.SentByUserName));
        }

        [Test]
        public Task Handle_PublishSentTransferConnectionInvitationEvent_SholdHaveTransferConnectionInvitationIdSet()
        {
            return CheckSentConnectionEvent((fixtures, message) => Assert.AreEqual(fixtures.TransferConnectionInvitation.Id, message.TransferConnectionInvitationId));
        }

        private async Task CheckHandler(
            Action<SendTransferConnectionInvitationHandlerTestFixtures> check = null,
            Action<SendTransferConnectionInvitationHandlerTestFixtures> setup = null)
        {
            var fixtures = new SendTransferConnectionInvitationHandlerTestFixtures()
                .SetSenderAccount()
                .SetReceiverAccount()
                .SetSenderUser();

            setup?.Invoke(fixtures);
            await fixtures.CallHandler();
            check?.Invoke(fixtures);
        }

        private async Task CheckSentConnectionEvent(
            Action<SendTransferConnectionInvitationHandlerTestFixtures, SentTransferConnectionInvitationEvent> checker)
        {
            SendTransferConnectionInvitationHandlerTestFixtures fixtures = null;
            await CheckHandler(setup: f => fixtures = f);
            var singleEvent = fixtures.GetSinglePublishedEvent<SentTransferConnectionInvitationEvent>();
            checker(fixtures, singleEvent);
        }
    }

    internal class SendTransferConnectionInvitationHandlerTestFixtures
    {
        public SendTransferConnectionInvitationHandlerTestFixtures()
        {
            EmployerAccountRepositoryMock = new Mock<IEmployerAccountRepository>();    
            PublicHashingServiceMock = new Mock<IPublicHashingService>();
            TransferConnectionInvitationRepositoryMock = new Mock<ITransferConnectionInvitationRepository>();
            UserRepositoryMock = new Mock<IUserRepository>();

            TransferConnectionInvitationRepositoryMock
                .Setup(r => r.Add(It.IsAny<TransferConnectionInvitation>()))
                .Returns(Task.CompletedTask).Callback<TransferConnectionInvitation>(c => Entity = TransferConnectionInvitation = c);
        }

        public Mock<IEmployerAccountRepository> EmployerAccountRepositoryMock { get; }
        public IEmployerAccountRepository EmployerAccountRepository => EmployerAccountRepositoryMock.Object;

        public Mock<IPublicHashingService> PublicHashingServiceMock { get; }
        public IPublicHashingService PublicHashingService => PublicHashingServiceMock.Object;

        public Mock<ITransferConnectionInvitationRepository> TransferConnectionInvitationRepositoryMock { get; }
        public ITransferConnectionInvitationRepository TransferConnectionInvitationRepository =>TransferConnectionInvitationRepositoryMock.Object;

        public Mock<IUserRepository> UserRepositoryMock { get; }
        public IUserRepository UserRepository => UserRepositoryMock.Object;

        public IEntity Entity { get; set; }
        public TransferConnectionInvitation TransferConnectionInvitation { get; set; }

        public User SenderUser { get; set; }

        public Domain.Data.Entities.Account.Account ReceiverAccount { get; set; }

        public Domain.Data.Entities.Account.Account  SenderAccount { get; set; }

        public long Result { get; set; }

        public SendTransferConnectionInvitationHandlerTestFixtures SetSenderUser()
        {
            SenderUser = new User
            {
                ExternalId = Guid.NewGuid(),
                Id = 123456,
                FirstName = "John",
                LastName = "Doe"
            };

            UserRepositoryMock
                .Setup(r => r.GetUserById(SenderUser.Id))
                .ReturnsAsync(SenderUser);

            return this;
        }

        public SendTransferConnectionInvitationHandlerTestFixtures SetReceiverAccount()
        {
            ReceiverAccount = new Domain.Data.Entities.Account.Account
            {
                Id = 222222,
                PublicHashedId = "XYZ987",
                Name = "Receiver"
            };

            return AddAccount(ReceiverAccount);
        }

        public SendTransferConnectionInvitationHandlerTestFixtures SetSenderAccount()
        {
            SenderAccount = new Domain.Data.Entities.Account.Account
            {
                Id = 333333,
                PublicHashedId = "ABC123",
                Name = "Sender"
            };

            return AddAccount(SenderAccount);
        }

        public SendTransferConnectionInvitationHandlerTestFixtures AddAccount(Domain.Data.Entities.Account.Account account)
        {
            EmployerAccountRepositoryMock.Setup(r => r.GetAccountById(account.Id)).ReturnsAsync(account);
            PublicHashingServiceMock.Setup(h => h.DecodeValue(account.PublicHashedId)).Returns(account.Id);

            return this;
        }

        public SendTransferConnectionInvitationCommandHandler CreateHandler()
        {
            return new SendTransferConnectionInvitationCommandHandler(
                EmployerAccountRepository,
                PublicHashingService,
                TransferConnectionInvitationRepository,
                UserRepository
            );
        }

        public Task<long> CallHandler()
        {
            var command = new SendTransferConnectionInvitationCommand
            {
                AccountId = SenderAccount.Id,
                UserId = SenderUser.Id,
                ReceiverAccountPublicHashedId = ReceiverAccount.PublicHashedId
            };

            return CreateHandler().Handle(command);
        }

        public SendTransferConnectionInvitationHandlerTestFixtures AddInvitationFromSenderToReceiver(TransferConnectionInvitationStatus status)
        {
            SenderAccount.SentTransferConnectionInvitations.Add(new TransferConnectionInvitationBuilder()
                .WithReceiverAccount(ReceiverAccount)
                .WithStatus(status)
                .Build());

            return this;
        }

        public TEventType GetSinglePublishedEvent<TEventType>()
        {
            return Entity.GetEvents().OfType<TEventType>().Single();
        }
    }
}