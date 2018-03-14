using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Data;
using SFA.DAS.EAS.Application.Mappings;
using SFA.DAS.EAS.Application.Queries.GetAccountTransferRole;
using SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionInvitation;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.TestCommon;
using SFA.DAS.EAS.TestCommon.Builders;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountTransferRoleTests
{ 
    [TestFixture]
    public class WhenIMakeAValidRequest
    {

        private List<TransferConnectionInvitation> _transferConnectionInvitations;

        private Dictionary<long, Domain.Data.Entities.Account.Account> _accounts;
        private GetAccountTransferRoleHandler _handler;

        [SetUp]
        public void Arrange()
        {
            _transferConnectionInvitations = new List<TransferConnectionInvitation>();
            _accounts = new Dictionary<long, Domain.Data.Entities.Account.Account>();

            var transferConnectionInvitationsDbSet = new DbSetStub<TransferConnectionInvitation>(_transferConnectionInvitations);
            var db = new Mock<EmployerAccountDbContext>();
            db.Setup(d => d.TransferConnectionInvitations).Returns(transferConnectionInvitationsDbSet);

            _handler = new GetAccountTransferRoleHandler(db.Object);
        }

        [TestCase(false, false)]
        [TestCase(false, false, TransferConnectionInvitationStatus.Rejected)]

        [TestCase(true, false, TransferConnectionInvitationStatus.Pending)]
        [TestCase(true, false, TransferConnectionInvitationStatus.Pending, TransferConnectionInvitationStatus.Rejected)]

        [TestCase(false, true, TransferConnectionInvitationStatus.Approved)]
        [TestCase(false, true, TransferConnectionInvitationStatus.Approved, TransferConnectionInvitationStatus.Rejected)]

        [TestCase(true, true, TransferConnectionInvitationStatus.Approved, TransferConnectionInvitationStatus.Pending)]
        [TestCase(true, true, TransferConnectionInvitationStatus.Approved, TransferConnectionInvitationStatus.Pending, TransferConnectionInvitationStatus.Rejected)]

        [TestCase(true, false, TransferConnectionInvitationStatus.Pending, TransferConnectionInvitationStatus.Pending, TransferConnectionInvitationStatus.Pending)]
        [TestCase(false, true, TransferConnectionInvitationStatus.Approved, TransferConnectionInvitationStatus.Approved, TransferConnectionInvitationStatus.Approved)]
        [TestCase(true, true, TransferConnectionInvitationStatus.Approved, TransferConnectionInvitationStatus.Approved, TransferConnectionInvitationStatus.Pending, TransferConnectionInvitationStatus.Pending)]
        public async Task ThenIsApprovedAndIsIsPendingShouldBeSetCorrectly(
            bool expectedIsPending, 
            bool expectedIsApproved, 
            params TransferConnectionInvitationStatus[] createTransfersWithStatus)
        {
            // Arrange
            const int receiverAccountId = 456;
            foreach (var status in createTransfersWithStatus)
            {
                AddTransfer(_senderAccountId++, receiverAccountId, status);
            }

            // Act
            var response = await _handler.Handle(BuildQuery(receiverAccountId));

            // Assert
            Assert.AreEqual(expectedIsPending, response.IsPendingReceiver, "Pending Receiver status is not the expected value");
            Assert.AreEqual(expectedIsApproved, response.IsApprovedReceiver, "Approved Receiver status is not the expected value");
        }

        private GetAccountTransferRoleQuery BuildQuery(int receiverAccountId)
        {
            return new GetAccountTransferRoleQuery
            {
                AccountId = receiverAccountId
            };
        }

        private void AddTransfer(long senderAccountId, long receiverAccountId, TransferConnectionInvitationStatus status)
        {
            var senderAccount = EnsureAccount(senderAccountId);
            var receiverAccount = EnsureAccount(receiverAccountId);

            var transferConnectionInvitation = new TransferConnectionInvitationBuilder()
                .WithId(_transferConnectionInvitations.Count)
                .WithSenderAccount(senderAccount)
                .WithReceiverAccount(receiverAccount)
                .WithStatus(status)
                .Build();

            _transferConnectionInvitations.Add(transferConnectionInvitation);
        }

        private Domain.Data.Entities.Account.Account EnsureAccount(long accountId)
        {
            if (!_accounts.TryGetValue(accountId, out var account))
            {
                account = new Domain.Data.Entities.Account.Account
                {
                    Id = accountId,
                    HashedId = $"ABC{_accounts.Count:D3}",
                    Name = $"Account {accountId}"
                };

                _accounts.Add(accountId, account);
            }

            return account;
        }

        private int _senderAccountId = 100;
    }
}