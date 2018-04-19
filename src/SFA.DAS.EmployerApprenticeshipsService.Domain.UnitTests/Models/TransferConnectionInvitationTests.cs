using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Models;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.TestCommon;
using SFA.DAS.EAS.TestCommon.Builders;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.UnitTests.Models
{
    [TestFixture]
    public class TransferConnectionInvitationTests : FluentTest<TransferConnectionInvitationTestsFixture>
    {
        [Test]
        public void SendTransferConnectionInvitation_WhenISendATransferConnection_ThenShouldCreateTransferConnectionInvitation()
        {
            Run(f => f.SendTransferConnectionInvitation(), f => f.TransferConnectionInvitation.Should().NotBeNull());
        }

        [Test]
        public void SendTransferConnectionInvitation_WhenISendATransferConnection_ThenShouldCreateTransferConnectionInvitationChange()
        {
            Run(f => f.SendTransferConnectionInvitation(), f => f.TransferConnectionInvitation.Changes.SingleOrDefault().Should().NotBeNull()
                .And.Match<TransferConnectionInvitationChange>(c =>
                    c.ReceiverAccount == f.ReceiverAccount && 
                    c.SenderAccount == f.SenderAccount &&
                    c.User == f.SenderUser));
        }

        [Test]
        public void SendTransferConnectionInvitation_WhenISendATransferConnection_ThenShouldPublishSentTransferConnectionInvitationEvent()
        {
            Run(f => f.SendTransferConnectionInvitation(), f => f.GetEvent<SentTransferConnectionInvitationEvent>().Should().NotBeNull()
                .And.Match<SentTransferConnectionInvitationEvent>(e =>
                    e.ReceiverAccountHashedId == f.ReceiverAccount.HashedId &&
                    e.ReceiverAccountId == f.ReceiverAccount.Id &&
                    e.ReceiverAccountName == f.ReceiverAccount.Name &&
                    e.SenderAccountHashedId == f.SenderAccount.HashedId &&
                    e.SenderAccountId == f.SenderAccount.Id &&
                    e.SenderAccountName == f.SenderAccount.Name &&
                    e.SentByUserExternalId == f.SenderUser.ExternalId &&
                    e.SentByUserId == f.SenderUser.Id &&
                    e.SentByUserName == f.SenderUser.FullName &&
                    e.TransferConnectionInvitationId == f.TransferConnectionInvitation.Id));
        }
    }

    public class TransferConnectionInvitationTestsFixture : FluentTestFixture
    {
        public TransferConnectionInvitation TransferConnectionInvitation { get; set; }
        public IEntity Entity { get; set; }
        public Account ReceiverAccount { get; set; }
        public long? Result { get; set; }
        public Account SenderAccount { get; set; }
        public decimal SenderAccountTransferAllowance { get; set; }
        public User SenderUser { get; set; }

        public TransferConnectionInvitationTestsFixture()
        {
            SetSenderAccount()
                .SetReceiverAccount()
                .SetSenderUser()
                .SetSenderAccountTransferAllowance(1);
        }

        public TransferConnectionInvitationTestsFixture AddInvitationFromSenderToReceiver(TransferConnectionInvitationStatus status)
        {
            SenderAccount.SentTransferConnectionInvitations.Add(
                new TransferConnectionInvitationBuilder()
                    .WithReceiverAccount(ReceiverAccount)
                    .WithStatus(status)
                    .Build());

            return this;
        }

        public TransferConnectionInvitationChange GetChange(int index)
        {
            return TransferConnectionInvitation.Changes.ElementAt(index);
        }

        public T GetEvent<T>()
        {
            return Entity.GetEvents().OfType<T>().SingleOrDefault();
        }

        public void SendTransferConnectionInvitation()
        {
            Entity = TransferConnectionInvitation = SenderAccount.SendTransferConnectionInvitation(ReceiverAccount, SenderUser, SenderAccountTransferAllowance);
        }

        public TransferConnectionInvitationTestsFixture SetSenderUser()
        {
            SenderUser = new User
            {
                ExternalId = Guid.NewGuid(),
                Id = 123456,
                FirstName = "John",
                LastName = "Doe"
            };

            return this;
        }

        public TransferConnectionInvitationTestsFixture SetReceiverAccount()
        {
            ReceiverAccount = new Account
            {
                Id = 222222,
                PublicHashedId = "XYZ987",
                Name = "Receiver"
            };

            return this;
        }

        public TransferConnectionInvitationTestsFixture SetSenderAccount()
        {
            SenderAccount = new Account
            {
                Id = 333333,
                PublicHashedId = "ABC123",
                Name = "Sender"
            };

            return this;
        }

        public TransferConnectionInvitationTestsFixture SetSenderAccountTransferAllowance(decimal transferAllowance)
        {
            SenderAccountTransferAllowance = transferAllowance;

            return this;
        }
    }
}