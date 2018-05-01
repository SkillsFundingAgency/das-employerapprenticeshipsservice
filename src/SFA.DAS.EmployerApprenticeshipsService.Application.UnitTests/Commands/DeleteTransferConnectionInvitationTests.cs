using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.DeleteSentTransferConnectionInvitation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.TestCommon;
using SFA.DAS.EAS.TestCommon.Builders;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.EAS.Application.UnitTests.Commands
{
    [TestFixture]
    public class DeleteTransferConnectionInvitationTests : FluentTest<DeleteTransferConnectionInvitationTestFixture>
    {
        [TestCase(DeleteTransferConnectionInvitationTestFixture.Constants.TestSenderAccountId)]
        [TestCase(DeleteTransferConnectionInvitationTestFixture.Constants.TestReceiverAccountId)]
        public void Handle_WhenMakingAValidCall_ThenShouldVerifyDeletingAccountExists(long deletingAccountId)
        {
            RunAsync(act: f => f.Handle(TransferConnectionInvitationStatus.Rejected, deletingAccountId), 
                assert: f => f.EmployerAccountRepositoryMock.Verify(r => r.GetAccountById(deletingAccountId), Times.Once));
        }

        [TestCase(DeleteTransferConnectionInvitationTestFixture.Constants.TestSenderAccountId)]
        [TestCase(DeleteTransferConnectionInvitationTestFixture.Constants.TestReceiverAccountId)]
        public void Handle_WhenMakingAValidCall_ThenShouldVerifyUserExists(long deletingAccountId)
        {
            RunAsync(act: f => f.Handle(TransferConnectionInvitationStatus.Rejected, deletingAccountId),
                assert: f => f.UserRepositoryMock.Verify(r => r.GetUserById(f.DeleterUser.Id), Times.Once));
        }

        [TestCase(DeleteTransferConnectionInvitationTestFixture.Constants.TestSenderAccountId)]
        [TestCase(DeleteTransferConnectionInvitationTestFixture.Constants.TestReceiverAccountId)]
        public void Handle_WhenMakingAValidCall_ThenShouldVerifyTransferConnectionInvitationExists(long deletingAccountId)
        {
            RunAsync(act: f => f.Handle(TransferConnectionInvitationStatus.Rejected, deletingAccountId),
                assert: f => f.TransferConnectionInvitationRepositoryMock.Verify(r => r.GetTransferConnectionInvitationById(f.TransferConnectionInvitation.Id), Times.Once));
        }

        [TestCase(DeleteTransferConnectionInvitationTestFixture.Constants.TestSenderAccountId)]
        [TestCase(DeleteTransferConnectionInvitationTestFixture.Constants.TestReceiverAccountId)]
        public void Handle_WhenMakingAValidCall_ThenInvitationShouldEndInRejectedStatus(long deletingAccountId)
        {
            RunAsync(act: f => f.Handle(TransferConnectionInvitationStatus.Rejected, deletingAccountId),
                assert: f => Assert.That(f.TransferConnectionInvitation.Status, Is.EqualTo(TransferConnectionInvitationStatus.Rejected)));
        }

        [Test]
        public void Handle_WhenSenderDeleting_ThenShouldBeDeletedBySender()
        {
            RunAsync(act: f => f.Handle(TransferConnectionInvitationStatus.Rejected, DeleteTransferConnectionInvitationTestFixture.Constants.TestSenderAccountId),
                assert: f =>Assert.That(f.TransferConnectionInvitation.DeletedBySender, Is.True));
        }

        [Test]
        public void Handle_WhenreceiverDeleting_ThenShouldBeDeletedByReceiver()
        {
            RunAsync(act: f => f.Handle(TransferConnectionInvitationStatus.Rejected, DeleteTransferConnectionInvitationTestFixture.Constants.TestSenderAccountId),
                assert: f => Assert.That(f.TransferConnectionInvitation.DeletedByReceiver, Is.True));
        }

        [TestCase(DeleteTransferConnectionInvitationTestFixture.Constants.TestSenderAccountId)]
        [TestCase(DeleteTransferConnectionInvitationTestFixture.Constants.TestReceiverAccountId)]
        public void Handle_WhenDeleting_ThenShouldBeOneChangeEntry(long deletingAccountId)
        {
            RunAsync(act: f => f.Handle(TransferConnectionInvitationStatus.Rejected, deletingAccountId),
                assert: f => Assert.That(f.TransferConnectionInvitation.Changes.Count, Is.EqualTo(1)));
        }

        [TestCase(DeleteTransferConnectionInvitationTestFixture.Constants.TestSenderAccountId)]
        [TestCase(DeleteTransferConnectionInvitationTestFixture.Constants.TestReceiverAccountId)]
        public void Handle_WhenSenderDeleting_ThenChangeEntryShouldBeCorrect(long deletingAccountId)
        {
            var now = DateTime.UtcNow;

            RunAsync(act: f => f.Handle(TransferConnectionInvitationStatus.Rejected, deletingAccountId),
                assert: f =>
                {
                    var change = f.TransferConnectionInvitation.Changes.Single();

                    Assert.That(change.CreatedDate, Is.GreaterThanOrEqualTo(now));
                    Assert.That(change.Status, Is.Null);
                    Assert.That(change.User, Is.Not.Null);
                    Assert.That(change.User.Id, Is.EqualTo(f.DeleterUser.Id));
                });
        }

        [TestCase(DeleteTransferConnectionInvitationTestFixture.Constants.TestSenderAccountId)]
        [TestCase(DeleteTransferConnectionInvitationTestFixture.Constants.TestReceiverAccountId)]
        public void Handle_WhenDeleting_ThenSingleEventShouldBeCreated(long deletingAccountId)
        {
            RunAsync(act: f => f.Handle(TransferConnectionInvitationStatus.Rejected, deletingAccountId),
                assert: f => Assert.That(f.Entity.GetEvents().OfType<DeletedTransferConnectionInvitationEvent>().Count(), Is.EqualTo(1)));
        }

        [TestCase(DeleteTransferConnectionInvitationTestFixture.Constants.TestSenderAccountId)]
        [TestCase(DeleteTransferConnectionInvitationTestFixture.Constants.TestReceiverAccountId)]
        public void Handle_WhenDeleting_ThenSingleEventShouldBeSetCorrectly(long deletingAccountId)
        {
            RunAsync(act: f => f.Handle(TransferConnectionInvitationStatus.Rejected, deletingAccountId),
                assert: f =>
                {
                    var message = f.Entity.GetEvents().OfType<DeletedTransferConnectionInvitationEvent>().Single();
                    Assert.That(message, Is.Not.Null);
                    Assert.That(message.DeletedByAccountId, Is.EqualTo(deletingAccountId));
                    Assert.That(message.DeletedByUserExternalId, Is.EqualTo(f.DeleterUser.ExternalId));
                    Assert.That(message.DeletedByUserId, Is.EqualTo(f.DeleterUser.Id));
                    Assert.That(message.DeletedByUserName, Is.EqualTo(f.DeleterUser.FullName));
                    Assert.That(message.CreatedAt,
                        Is.EqualTo(f.TransferConnectionInvitation.Changes.Select(c => c.CreatedDate).Cast<DateTime?>()
                            .SingleOrDefault()));
                    Assert.That(message.ReceiverAccountHashedId, Is.EqualTo(f.ReceiverAccount.HashedId));
                    Assert.That(message.ReceiverAccountId, Is.EqualTo(f.ReceiverAccount.Id));
                    Assert.That(message.ReceiverAccountName, Is.EqualTo(f.ReceiverAccount.Name));
                    Assert.That(message.SenderAccountHashedId, Is.EqualTo(f.SenderAccount.HashedId));
                    Assert.That(message.SenderAccountId, Is.EqualTo(f.SenderAccount.Id));
                    Assert.That(message.SenderAccountName, Is.EqualTo(f.SenderAccount.Name));
                    Assert.That(message.TransferConnectionInvitationId, Is.EqualTo(f.TransferConnectionInvitation.Id));
                });
        }

        public void Handle_WhenSenderDeleting_ThenShouldLookLikeDeletedBySender()
        {
            RunAsync(act: f => f.Handle(TransferConnectionInvitationStatus.Rejected, DeleteTransferConnectionInvitationTestFixture.Constants.TestSenderAccountId),
                assert: f =>
                {
                    Assert.IsTrue(f.TransferConnectionInvitation.DeletedBySender);
                    Assert.IsFalse(f.TransferConnectionInvitation.DeletedByReceiver);
                    Assert.IsTrue(f.TransferConnectionInvitation.Changes.SingleOrDefault(tcic => 
                                      tcic.DeletedBySender.HasValue && 
                                      tcic.DeletedBySender.Value && !tcic.DeletedByReceiver.HasValue) != null);
                });
        }

        public void Handle_WhenReceiverDeleting_ThenShouldLookLikeDeletedByReceiver()
        {
            RunAsync(act: f => f.Handle(TransferConnectionInvitationStatus.Rejected, DeleteTransferConnectionInvitationTestFixture.Constants.TestReceiverAccountId),
                assert: f =>
                {
                    Assert.IsTrue(f.TransferConnectionInvitation.DeletedByReceiver);
                    Assert.IsFalse(f.TransferConnectionInvitation.DeletedBySender);
                    Assert.IsTrue(f.TransferConnectionInvitation.Changes.SingleOrDefault(tcic =>
                                      tcic.DeletedByReceiver.HasValue && 
                                      tcic.DeletedByReceiver.Value &&
                                      !tcic.DeletedBySender.HasValue) != null);
                });
        }

        [TestCase(DeleteTransferConnectionInvitationTestFixture.Constants.TestSenderAccountId, TransferConnectionInvitationStatus.Approved)]
        [TestCase(DeleteTransferConnectionInvitationTestFixture.Constants.TestReceiverAccountId, TransferConnectionInvitationStatus.Approved)]
        [TestCase(DeleteTransferConnectionInvitationTestFixture.Constants.TestSenderAccountId, TransferConnectionInvitationStatus.Pending)]
        [TestCase(DeleteTransferConnectionInvitationTestFixture.Constants.TestReceiverAccountId, TransferConnectionInvitationStatus.Pending)]
        public void Handle_WhenDeleting_ThenShouldThrowExceptionIfNotRejected(long deletingAccountId, TransferConnectionInvitationStatus status)
        {
            Assert.ThrowsAsync<Exception>(() =>
                RunAsync(
                    act: f => f.Handle(status, deletingAccountId),
                    assert: null), "Requires transfer connection invitation is rejected.");
        }
    }

    public class DeleteTransferConnectionInvitationTestFixture : FluentTestFixture
    {
        public DeleteTransferConnectionInvitationTestFixture()
        {
            EmployerAccountRepositoryMock = new Mock<IEmployerAccountRepository>();
            TransferConnectionInvitationRepositoryMock = new Mock<ITransferConnectionInvitationRepository>();
            UserRepositoryMock = new Mock<IUserRepository>();
        }

        public Mock<IEmployerAccountRepository> EmployerAccountRepositoryMock;
        public IEmployerAccountRepository EmployerAccountRepository => EmployerAccountRepositoryMock.Object;

        public Mock<ITransferConnectionInvitationRepository> TransferConnectionInvitationRepositoryMock;
        public ITransferConnectionInvitationRepository TransferConnectionInvitationRepository => TransferConnectionInvitationRepositoryMock.Object;

        public Mock<IUserRepository> UserRepositoryMock;
        public IUserRepository UserRepository => UserRepositoryMock.Object;

        public Domain.Models.Account.Account SenderAccount { get; private set; }
        public Domain.Models.Account.Account ReceiverAccount { get; private set; }
        public User DeleterUser { get; private set; }
        public TransferConnectionInvitation TransferConnectionInvitation { get; private set; }
        public IEntity Entity { get; private set; }

        public DeleteTransferConnectionInvitationTestFixture WithSenderAccount(long senderAccountId)
        {
            SenderAccount = new Domain.Models.Account.Account
            {
                Id = senderAccountId,
                HashedId = "ABC123",
                Name = "Sender"
            };

            EmployerAccountRepositoryMock
                .Setup(r => r.GetAccountById(SenderAccount.Id))
                .ReturnsAsync(SenderAccount);

            return this;
        }

        public DeleteTransferConnectionInvitationTestFixture WithReceiverAccount(long receiverAccountId)
        {
            ReceiverAccount = new Domain.Models.Account.Account
            {
                Id = receiverAccountId,
                HashedId = "ABC123",
                Name = "Receiver"
            };

            EmployerAccountRepositoryMock
                .Setup(r => r.GetAccountById(ReceiverAccount.Id))
                .ReturnsAsync(ReceiverAccount);

            return this;
        }

        public DeleteTransferConnectionInvitationTestFixture WithDeleterUser(long deletedUserId)
        {
            DeleterUser = new User
            {
                Id = deletedUserId,
                ExternalId = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe"
            };

            UserRepositoryMock
                .Setup(r => r.GetUserById(DeleterUser.Id))
                .ReturnsAsync(DeleterUser);

            return this;
        }

        public DeleteTransferConnectionInvitationTestFixture WithTransferConnection(
            TransferConnectionInvitationStatus status)
        {
            Entity = TransferConnectionInvitation = new TransferConnectionInvitationBuilder()
                .WithId(111111)
                .WithSenderAccount(SenderAccount)
                .WithReceiverAccount(ReceiverAccount)
                .WithStatus(status)
                .Build();

            TransferConnectionInvitationRepositoryMock
                .Setup(r => r.GetTransferConnectionInvitationById(TransferConnectionInvitation.Id))
                .ReturnsAsync(TransferConnectionInvitation);

            return this;
        }

        public static class Constants
        {
            public const long TestSenderAccountId = 123;
            public const long TestReceiverAccountId = 456;
            public const long TestUserId = 789;
        }

        public long TestSenderAccountId => Constants.TestSenderAccountId;

        public long TestReceiverAccountId => Constants.TestReceiverAccountId;

        public long TestUserId => Constants.TestUserId;

        public Task Handle(TransferConnectionInvitationStatus status, long deletingAccountId)
        {
            WithSenderAccount(TestSenderAccountId)
                .WithReceiverAccount(TestReceiverAccountId)
                .WithDeleterUser(TestUserId)
                .WithTransferConnection(status);

            var command = new DeleteTransferConnectionInvitationCommand
            {
                AccountId = deletingAccountId,
                UserId = DeleterUser.Id,
                TransferConnectionInvitationId = TransferConnectionInvitation.Id
            };

            var handler = CreateHandler();

            return handler.Handle(command);
        }

        private DeleteTransferConnectionInvitationCommandHandler CreateHandler()
        {
            return new DeleteTransferConnectionInvitationCommandHandler(
                EmployerAccountRepository,
                TransferConnectionInvitationRepository,
                UserRepository
            );
        }
    }
}