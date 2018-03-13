using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.UnitTests.Data.Entities.AccountTests
{
    [TestFixture]
    public class WhenISendTransferConnection
    {
        [Test]
        public void ShouldRejectIfReceiverHasSentAPendingInvite()
        {
            // Arrange
            var someReceiver = AccountBuilder.Create();
            var receiver = AccountBuilder.Create().WithInvitationTo(someReceiver, TransferConnectionInvitationStatus.Pending);
            var sender = new Account();

            // Assert
            Assert.Throws<Exception>(() => sender.SendTransferConnectionInvitation(receiver, UserBuiler.Create()));
        }

        [Test]
        public void ShouldRejectIfReceiverHasSentAnAcceptedInvite()
        {
            // Arrange
            var someReceiver = AccountBuilder.Create();
            var receiver = AccountBuilder.Create().WithInvitationTo(someReceiver, TransferConnectionInvitationStatus.Approved);

            var sender = new Account();

            // Assert
            Assert.Throws<Exception>(() => sender.SendTransferConnectionInvitation(receiver, UserBuiler.Create()));
        }

        [Test]
        public void ShouldNotRejectIfReceiverHasSentARejectedInvite()
        {
            // Arrange
            var someReceiver = AccountBuilder.Create();
            var receiver = AccountBuilder.Create().WithInvitationTo(someReceiver, TransferConnectionInvitationStatus.Rejected);
            var sender = new Account();

            // Act
            sender.SendTransferConnectionInvitation(receiver, UserBuiler.Create());

            // Assert
            Assert.Pass("Should not have got an exception");
        }

        [Test]
        public void ShouldAddNewTransferRequestIfReceiverHasSentARejectedInvite()
        {
            // Arrange
            var someReceiver = AccountBuilder.Create();
            var receiver = AccountBuilder.Create().WithInvitationTo(someReceiver, TransferConnectionInvitationStatus.Rejected);
            var sender = new Account();

            // Act
            sender.SendTransferConnectionInvitation(receiver, UserBuiler.Create());

            // Assert
            const int expectedNumberOfTransfers = 1;
            Assert.AreEqual(expectedNumberOfTransfers, sender.SentTransferConnectionInvitations.Count);
        }

        [Test]
        public void CreatedTransferConnectionInvitation_IsAcceptedRequest_ShouldSetCreatedDate()
        {
            var now = DateTime.UtcNow;
            CheckCreatedTransferConnectionInvitation(tci => Assert.That(tci.CreatedDate, Is.GreaterThanOrEqualTo(now)));
        }

        [Test]
        public void CreatedTransferConnectionInvitation_IsAcceptedRequest_ShouldSetDeletedByReceiver()
        {
            CheckCreatedTransferConnectionInvitation(tci => Assert.That(tci.DeletedByReceiver, Is.False));
        }

        [Test]
        public void CreatedTransferConnectionInvitation_IsAcceptedRequest_ShouldSetDeletredBySender()
        {
            CheckCreatedTransferConnectionInvitation(tci => Assert.That(tci.DeletedBySender, Is.False));
        }

        [Test]
        public void CreatedTransferConnectionInvitation_IsAcceptedRequest_ShouldSetReceiver()
        {
            CheckCreatedTransferConnectionInvitation(checkAccounts: (tci, sender, receiver) => Assert.That(tci.ReceiverAccount, Is.SameAs(receiver)));
        }

        [Test]
        public void CreatedTransferConnectionInvitation_IsAcceptedRequest_ShouldSetSender()
        {
            CheckCreatedTransferConnectionInvitation(checkAccounts: (tci, sender, receiver) => Assert.That(tci.SenderAccount, Is.SameAs(sender)));
        }

        [Test]
        public void CreatedTransferConnectionInvitation_IsAcceptedRequest_ShouldSetPending()
        {
            CheckCreatedTransferConnectionInvitation(tci => Assert.That(tci.Status, Is.EqualTo(TransferConnectionInvitationStatus.Pending)));
        }

        [Test]
        public void CreatedTransferConnectionInvitation_IsAcceptedRequest_ShouldSetChangesCount()
        {
            CheckCreatedTransferConnectionInvitation(tci => Assert.That(tci.Changes.Count, Is.EqualTo(1)));
        }

        [Test]
        public void CreatedTransferConnectionInvitation_IsAcceptedRequest_ShouldSetChangedDate()
        {
            CheckCreatedTransferConnectionInvitation(checkChange: (tci, change) => Assert.That(change.CreatedDate, Is.EqualTo(tci.CreatedDate)));
        }

        [Test]
        public void CreatedTransferConnectionInvitation_IsAcceptedRequest_ShouldSetChangedDeletedByReceiver()
        {
            CheckCreatedTransferConnectionInvitation(checkChange: (tci, change) => Assert.That(change.DeletedByReceiver, Is.EqualTo(tci.DeletedByReceiver)));
        }

        [Test]
        public void CreatedTransferConnectionInvitation_IsAcceptedRequest_ShouldSetChangedDeletedBySender()
        {
            CheckCreatedTransferConnectionInvitation(checkChange: (tci, change) => Assert.That(change.DeletedBySender, Is.EqualTo(tci.DeletedBySender)));
        }

        [Test]
        public void CreatedTransferConnectionInvitation_IsAcceptedRequest_ShouldSetChangedReceiverAccount()
        {
            CheckCreatedTransferConnectionInvitation(checkChange: (tci, change) => Assert.That(change.ReceiverAccount, Is.SameAs(tci.ReceiverAccount)));
        }

        [Test]
        public void CreatedTransferConnectionInvitation_IsAcceptedRequest_ShouldSetChangedSenderAccount()
        {
            CheckCreatedTransferConnectionInvitation(checkChange: (tci, change) => Assert.That(change.SenderAccount, Is.SameAs(tci.SenderAccount)));
        }

        [Test]
        public void CreatedTransferConnectionInvitation_IsAcceptedRequest_ShouldSetChangedStatus()
        {
            CheckCreatedTransferConnectionInvitation(checkChange: (tci, change) => Assert.That(change.Status, Is.EqualTo(tci.Status)));
        }

        [Test]
        public void CreatedTransferConnectionInvitation_IsAcceptedRequest_ShouldSetChangedUser()
        {
            CheckCreatedTransferConnectionInvitation(checkChange: (tci, change) => Assert.That(change.User, Is.Not.Null));
        }

        [Test]
        public void CreatedTransferConnectionInvitation_IsAcceptedRequest_ShouldSetChangedUserId()
        {
            CheckCreatedTransferConnectionInvitation(checkChange: (tci, change) => Assert.That(change.User.Id, Is.EqualTo(tci.SenderAccountId)));
        }

        private void CheckCreatedTransferConnectionInvitation(
            Action<TransferConnectionInvitation> check = null,
            Action<TransferConnectionInvitation, Account, Account> checkAccounts = null,
            Action<TransferConnectionInvitation, TransferConnectionInvitationChange> checkChange = null)
        {
            // Arrange
            var receiver = AccountBuilder.Create();
            var sender = new Account();
            var user = UserBuiler.Create();
            var now = DateTime.UtcNow;

            // Act
            var transferConnectionInvitation = sender.SendTransferConnectionInvitation(receiver, user);

            check?.Invoke(transferConnectionInvitation);
            checkAccounts?.Invoke(transferConnectionInvitation, sender, receiver);
            checkChange?.Invoke(transferConnectionInvitation, transferConnectionInvitation.Changes.Single());
        }
    }

    public static class UserBuiler
    {
        public static User Create()
        {
            return new User();
        }
    }

    public static class AccountBuilder
    {
        private static int _accountId = 1;

        public static Account Create()
        {
            return new Account
            {
                Id = Interlocked.Increment(ref _accountId)
            };
        }

        public static Account WithInvitationTo(this Account account, Account receiver, TransferConnectionInvitationStatus status)
        {
            var tci = new Mock<TransferConnectionInvitation>();
            tci.SetupProperty(t => t.ReceiverAccountId, receiver.Id);
            tci.SetupProperty(t => t.SenderAccountId, account.Id);
            tci.SetupProperty(t => t.Status, status);

            receiver.ReceivedTransferConnectionInvitation.Add(tci.Object);
            account.SentTransferConnectionInvitations.Add(tci.Object);

            return account;
        }
    }
}
