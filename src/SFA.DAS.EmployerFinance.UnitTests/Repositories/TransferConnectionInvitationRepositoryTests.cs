using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.EmployerFinance.Models.TransferConnections;
using SFA.DAS.EmployerFinance.TestCommon.Builders;
using SFA.DAS.Testing;
using SFA.DAS.Testing.EntityFramework;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetLatestPendingReceivedTransferConnectionInvitationTests
{
    [TestFixture]
    public class TransferConnectionInvitationRepositoryTests : FluentTest<TransferConnectionInvitationRepositoryTestFixture>
    {
        [Test]
        public void WhenIGetLatestPendingByReceiver_ThenShouldReturnCorrectInvitation()
        {
            RunAsync(f => f.GetLatestPendingByReceiver(), f => f.TransferConnectionInvitation.Should().NotBeNull()
                .And.Match<TransferConnectionInvitation>(r =>
                    r != null &&
                    r.Id == f.PendingReceivedTransferConnectionInvitation2.Id &&
                    r.SenderAccount.Id == f.SenderAccount1.Id &&
                    r.ReceiverAccount.Id == f.ReceiverAccount2.Id &&
                    r.Status == f.PendingReceivedTransferConnectionInvitation2.Status &&
                    r.CreatedDate == f.PendingReceivedTransferConnectionInvitation2.CreatedDate));
        }

        [Test]
        public void WhenIGetByReceiver_ThenShouldReturnCorrectInvitation()
        {
            RunAsync(f => f.GetByReceiver(1, f.ReceiverAccount2.Id, TransferConnectionInvitationStatus.Pending), f => f.TransferConnectionInvitation.Should().NotBeNull()
                .And.Match<TransferConnectionInvitation>(r =>
                    r != null &&
                    r.Id == f.PendingReceivedTransferConnectionInvitation1.Id &&
                    r.SenderAccount.Id == f.SenderAccount1.Id &&
                    r.ReceiverAccount.Id == f.ReceiverAccount2.Id &&
                    r.Status == f.PendingReceivedTransferConnectionInvitation1.Status &&
                    r.CreatedDate == f.PendingReceivedTransferConnectionInvitation1.CreatedDate));
        }

        [Test]
        public void WhenIGetBySender_ThenShouldReturnCorrectInvitation()
        {
            RunAsync(f => f.GetByReceiver(3, f.SenderAccount1.Id, TransferConnectionInvitationStatus.Pending), f => f.TransferConnectionInvitation.Should().NotBeNull()
                .And.Match<TransferConnectionInvitation>(r =>
                    r != null &&
                    r.Id == f.PendingReceivedTransferConnectionInvitation3.Id &&
                    r.SenderAccount.Id == f.SenderAccount1.Id &&
                    r.ReceiverAccount.Id == f.ReceiverAccount2.Id &&
                    r.Status == f.PendingReceivedTransferConnectionInvitation3.Status &&
                    r.CreatedDate == f.PendingReceivedTransferConnectionInvitation3.CreatedDate));
        }

        [Test]
        public void WhenIGetBySenderOrRecevier_ThenShouldReturnSingleCorrectInvitation()
        {
            RunAsync(f => f.GetBySenderOrReceiver(4, f.SenderAccount3.Id), f => f.TransferConnectionInvitation.Should().NotBeNull()
                .And.Match<TransferConnectionInvitation>(r =>
                    r != null &&
                    r.Id == f.PendingReceivedTransferConnectionInvitation4.Id &&
                    r.SenderAccount.Id == f.SenderAccount3.Id &&
                    r.ReceiverAccount.Id == f.ReceiverAccount4.Id &&
                    r.Status == f.PendingReceivedTransferConnectionInvitation4.Status &&
                    r.CreatedDate == f.PendingReceivedTransferConnectionInvitation4.CreatedDate));

            RunAsync(f => f.GetBySenderOrReceiver(4, f.ReceiverAccount4.Id), f => f.TransferConnectionInvitation.Should().NotBeNull()
                .And.Match<TransferConnectionInvitation>(r =>
                    r != null &&
                    r.Id == f.PendingReceivedTransferConnectionInvitation4.Id &&
                    r.SenderAccount.Id == f.SenderAccount3.Id &&
                    r.ReceiverAccount.Id == f.ReceiverAccount4.Id &&
                    r.Status == f.PendingReceivedTransferConnectionInvitation4.Status &&
                    r.CreatedDate == f.PendingReceivedTransferConnectionInvitation4.CreatedDate));
        }

        [Test]
        public void WhenIGetBySenderOrRecevier_ThenShouldReturnMultipleCorrectInvitations()
        {
            RunAsync(f => f.GetBySenderOrReceiver(f.SenderAccount1.Id), f => f.TransferConnectionInvitations.Should().NotBeNull()
                .And.Match<List<TransferConnectionInvitation>>(r =>
                    r != null &&
                    r.Count == 4));

            RunAsync(f => f.GetBySenderOrReceiver(f.ReceiverAccount2.Id), f => f.TransferConnectionInvitations.Should().NotBeNull()
                .And.Match<List<TransferConnectionInvitation>>(r =>
                    r != null &&
                    r.Count == 4));
        }
    }

    public class TransferConnectionInvitationRepositoryTestFixture : FluentTestFixture
    {
        public TransferConnectionInvitationRepository Sut { get; set; }
        public Mock<EmployerFinanceDbContext> Db { get; set; }

        public TransferConnectionInvitation PendingReceivedTransferConnectionInvitation1 { get; set; }
        public TransferConnectionInvitation PendingReceivedTransferConnectionInvitation2 { get; set; }
        public TransferConnectionInvitation PendingReceivedTransferConnectionInvitation3 { get; set; }
        public TransferConnectionInvitation PendingReceivedTransferConnectionInvitation4 { get; set; }
        public TransferConnectionInvitation ApprovedReceivedTransferConnectionInvitation { get; set; }
        
        public Account SenderAccount1 { get; set; }
        public Account ReceiverAccount2 { get; set; }
        public Account SenderAccount3 { get; set; }
        public Account ReceiverAccount4 { get; set; }

        public TransferConnectionInvitation TransferConnectionInvitation { get; set; }
        public List<TransferConnectionInvitation> TransferConnectionInvitations { get; set; }

        public TransferConnectionInvitationRepositoryTestFixture()
        {
            SenderAccount1 = new Account
            {
                Id = 1,
                Name = "Sender1"
            };

            ReceiverAccount2 = new Account
            {
                Id = 2,
                Name = "Receiver2"
            };

            SenderAccount3 = new Account
            {
                Id = 3,
                Name = "Sender3"
            };

            ReceiverAccount4 = new Account
            {
                Id = 4,
                Name = "Receiver4"
            };

            PendingReceivedTransferConnectionInvitation1 = new TransferConnectionInvitationBuilder()
                .WithId(1)
                .WithSenderAccount(SenderAccount1)
                .WithReceiverAccount(ReceiverAccount2)
                .WithStatus(TransferConnectionInvitationStatus.Pending)
                .WithCreatedDate(DateTime.UtcNow.AddDays(-1))
                .Build();

            PendingReceivedTransferConnectionInvitation2 = new TransferConnectionInvitationBuilder()
                .WithId(2)
                .WithSenderAccount(SenderAccount1)
                .WithReceiverAccount(ReceiverAccount2)
                .WithStatus(TransferConnectionInvitationStatus.Pending)
                .WithCreatedDate(DateTime.UtcNow)
                .Build();

            PendingReceivedTransferConnectionInvitation3 = new TransferConnectionInvitationBuilder()
                .WithId(3)
                .WithSenderAccount(SenderAccount1)
                .WithReceiverAccount(ReceiverAccount2)
                .WithStatus(TransferConnectionInvitationStatus.Pending)
                .WithCreatedDate(DateTime.UtcNow.AddDays(-2))
                .Build();

            PendingReceivedTransferConnectionInvitation4 = new TransferConnectionInvitationBuilder()
                .WithId(1)
                .WithSenderAccount(SenderAccount3)
                .WithReceiverAccount(ReceiverAccount4)
                .WithStatus(TransferConnectionInvitationStatus.Pending)
                .WithCreatedDate(DateTime.UtcNow)
                .Build();

            ApprovedReceivedTransferConnectionInvitation = new TransferConnectionInvitationBuilder()
                .WithId(4)
                .WithSenderAccount(SenderAccount3)
                .WithReceiverAccount(ReceiverAccount4)
                .WithStatus(TransferConnectionInvitationStatus.Approved)
                .Build();

            var data = new List<TransferConnectionInvitation>
            {
                PendingReceivedTransferConnectionInvitation1,
                PendingReceivedTransferConnectionInvitation2,
                PendingReceivedTransferConnectionInvitation3,
                ApprovedReceivedTransferConnectionInvitation
            };

            Db.Setup(d => d.TransferConnectionInvitations).Returns(new DbSetStub<TransferConnectionInvitation>(data));
        }

        public async Task GetLatestPendingByReceiver()
        {
            TransferConnectionInvitation = await Sut.GetLatestByReceiver(ReceiverAccount4.Id, TransferConnectionInvitationStatus.Pending);
        }

        public async Task GetByReceiver(int transferConnectionInvitationId, long receiverAccountId, TransferConnectionInvitationStatus status)
        {
            TransferConnectionInvitation = await Sut.GetByReceiver(transferConnectionInvitationId, receiverAccountId, status);
        }

        public async Task GetBySender(int transferConnectionInvitationId, long senderAccountId, TransferConnectionInvitationStatus status)
        {
            TransferConnectionInvitation = await Sut.GetByReceiver(transferConnectionInvitationId, senderAccountId, status);
        }

        public async Task GetBySenderOrReceiver(long accountId)
        {
            TransferConnectionInvitations = await Sut.GetBySenderOrReceiver(accountId);
        }

        public async Task GetBySenderOrReceiver(int transferConnectionInvitationId, long accountId)
        {
            TransferConnectionInvitation = await Sut.GetBySenderOrReceiver(transferConnectionInvitationId, accountId);
        }
    }
}
