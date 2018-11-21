using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Mappings;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.TransferConnections;
using SFA.DAS.EmployerAccounts.Queries.GetTransferConnections;
using SFA.DAS.EmployerAccounts.UnitTests.Builders;
using SFA.DAS.Testing.EntityFramework;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetTransferConnectionsTests
{
    [TestFixture]
    public class WhenIGetTransferConnections
    {
        private GetTransferConnectionsQueryHandler _handler;
        private GetTransferConnectionsQuery _query;
        private GetTransferConnectionsResponse _response;
        private Mock<EmployerAccountsDbContext> _db;
        private DbSetStub<TransferConnectionInvitation> _transferConnectionInvitationsDbSet;
        private List<TransferConnectionInvitation> _transferConnectionInvitations;
        private TransferConnectionInvitation _sentTransferConnectionInvitation;
        private TransferConnectionInvitation _receivedTransferConnectionInvitation1;
        private TransferConnectionInvitation _receivedTransferConnectionInvitation2;
        private TransferConnectionInvitation _receivedTransferConnectionInvitation3;
        private Account _senderAccount1;
        private Account _senderAccount2;
        private Account _receiverAccount;
        private IConfigurationProvider _configurationProvider;

        [SetUp]
        public void Arrange()
        {
            _db = new Mock<EmployerAccountsDbContext>();

            _senderAccount1 = new Account
            {
                Id = 111111,
                HashedId = "ABC123",
                PublicHashedId = "321CBA",
                Name = "Sender 1"
            };

            _senderAccount2 = new Account
            {
                Id = 222222,
                HashedId = "DEF456",
                PublicHashedId = "654FED",
                Name = "Sender 2"
            };

            _receiverAccount = new Account
            {
                Id = 333333,
                HashedId = "GHI789",
                PublicHashedId = "987IHG",
                Name = "Receiver"
            };

            _sentTransferConnectionInvitation = new TransferConnectionInvitationBuilder()
                .WithId(111111)
                .WithSenderAccount(_receiverAccount)
                .WithReceiverAccount(new Account())
                .WithStatus(TransferConnectionInvitationStatus.Approved)
                .Build();

            _receivedTransferConnectionInvitation1 = new TransferConnectionInvitationBuilder()
                .WithId(222222)
                .WithSenderAccount(_senderAccount1)
                .WithReceiverAccount(_receiverAccount)
                .WithStatus(TransferConnectionInvitationStatus.Rejected)
                .Build();

            _receivedTransferConnectionInvitation2 = new TransferConnectionInvitationBuilder()
                .WithId(333333)
                .WithSenderAccount(_senderAccount1)
                .WithReceiverAccount(_receiverAccount)
                .WithStatus(TransferConnectionInvitationStatus.Approved)
                .Build();

            _receivedTransferConnectionInvitation3 = new TransferConnectionInvitationBuilder()
                .WithId(444444)
                .WithSenderAccount(_senderAccount2)
                .WithReceiverAccount(_receiverAccount)
                .WithStatus(TransferConnectionInvitationStatus.Approved)
                .Build();

            _transferConnectionInvitations = new List<TransferConnectionInvitation>
            {
                _sentTransferConnectionInvitation,
                _receivedTransferConnectionInvitation3,
                _receivedTransferConnectionInvitation2,
                _receivedTransferConnectionInvitation1,
                new TransferConnectionInvitationBuilder()
                    .WithSenderAccount(new Account())
                    .WithReceiverAccount(new Account())
                    .WithStatus(TransferConnectionInvitationStatus.Approved)
                    .Build()
            };

            _transferConnectionInvitationsDbSet = new DbSetStub<TransferConnectionInvitation>(_transferConnectionInvitations);

            _configurationProvider = new MapperConfiguration(c =>
            {
                c.AddProfile<TransferConnectionInvitationMappings>();
            });

            _db.Setup(d => d.TransferConnectionInvitations).Returns(_transferConnectionInvitationsDbSet);

            _handler = new GetTransferConnectionsQueryHandler(new Lazy<EmployerAccountsDbContext>(() => _db.Object), _configurationProvider);

            _query = new GetTransferConnectionsQuery
            {
                AccountId = _receiverAccount.Id
            };
        }

        [Test]
        public async Task ThenShouldReturnGetTransferConnectionInvitationsResponse()
        {
            _response = await _handler.Handle(_query);

            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.TransferConnections.Count(), Is.EqualTo(2));

            var transferConnectionInvitation1 = _response.TransferConnections.ElementAt(0);
            
            Assert.That(transferConnectionInvitation1.FundingEmployerAccountId, Is.EqualTo(_senderAccount1.Id));
            Assert.That(transferConnectionInvitation1.FundingEmployerHashedAccountId, Is.EqualTo(_senderAccount1.HashedId));
            Assert.That(transferConnectionInvitation1.FundingEmployerPublicHashedAccountId, Is.EqualTo(_senderAccount1.PublicHashedId));
            Assert.That(transferConnectionInvitation1.FundingEmployerAccountName, Is.EqualTo(_senderAccount1.Name));

            var transferConnectionInvitation2 = _response.TransferConnections.ElementAt(1);
            
            Assert.That(transferConnectionInvitation2.FundingEmployerAccountId, Is.EqualTo(_senderAccount2.Id));
            Assert.That(transferConnectionInvitation2.FundingEmployerHashedAccountId, Is.EqualTo(_senderAccount2.HashedId));
            Assert.That(transferConnectionInvitation2.FundingEmployerPublicHashedAccountId, Is.EqualTo(_senderAccount2.PublicHashedId));
            Assert.That(transferConnectionInvitation2.FundingEmployerAccountName, Is.EqualTo(_senderAccount2.Name));
        }
    }
}