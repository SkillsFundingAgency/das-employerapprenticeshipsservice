﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Mappings;
using SFA.DAS.EmployerFinance.Models.TransferConnections;
using SFA.DAS.EmployerFinance.Queries.GetTransferConnectionInvitations;
using SFA.DAS.EmployerFinance.UnitTests.Builders;
using SFA.DAS.Testing.EntityFramework;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetTransferConnectionInvitationsTests
{
    [TestFixture]
    public class WhenIGetTransferConnectionInvitations
    {
        private GetTransferConnectionInvitationsQueryHandler _handler;
        private GetTransferConnectionInvitationsQuery _query;
        private GetTransferConnectionInvitationsResponse _response;
        private Mock<EmployerAccountsDbContext> _db;
        private DbSetStub<TransferConnectionInvitation> _transferConnectionInvitationsDbSet;
        private List<TransferConnectionInvitation> _transferConnectionInvitations;
        private TransferConnectionInvitation _sentTransferConnectionInvitation;
        private TransferConnectionInvitation _receivedTransferConnectionInvitation;
        private EmployerFinance.Models.Account.Account _account;
        private IConfigurationProvider _configurationProvider;

        [SetUp]
        public void Arrange()
        {
            _db = new Mock<EmployerAccountsDbContext>();

            _account = new EmployerFinance.Models.Account.Account
            {
                Id = 333333,
                HashedId = "ABC123",
                Name = "Account"
            };

            _sentTransferConnectionInvitation = new TransferConnectionInvitationBuilder()
                .WithId(222222)
                .WithSenderAccount(_account)
                .WithReceiverAccount(new EmployerFinance.Models.Account.Account())
                .WithCreatedDate(DateTime.UtcNow)
                .Build();

            _receivedTransferConnectionInvitation = new TransferConnectionInvitationBuilder()
                .WithId(111111)
                .WithSenderAccount(new EmployerFinance.Models.Account.Account())
                .WithReceiverAccount(_account)
                .WithCreatedDate(DateTime.UtcNow.AddDays(-1))
                .Build();

            _transferConnectionInvitations = new List<TransferConnectionInvitation>
            {
                _sentTransferConnectionInvitation,
                _receivedTransferConnectionInvitation,
                new TransferConnectionInvitationBuilder()
                    .WithSenderAccount(new EmployerFinance.Models.Account.Account())
                    .WithReceiverAccount(new EmployerFinance.Models.Account.Account())
                    .WithCreatedDate(DateTime.UtcNow.AddDays(-2))
                    .Build()
            };

            _transferConnectionInvitationsDbSet = new DbSetStub<TransferConnectionInvitation>(_transferConnectionInvitations);

            _configurationProvider = new MapperConfiguration(c =>
            {
                c.AddProfile<AccountMappings>();
                c.AddProfile<TransferConnectionInvitationMappings>();
                c.AddProfile<UserMappings>();
            });

            _db.Setup(d => d.TransferConnectionInvitations).Returns(_transferConnectionInvitationsDbSet);

            _handler = new GetTransferConnectionInvitationsQueryHandler(new Lazy<EmployerAccountsDbContext>(() => _db.Object), _configurationProvider);

            _query = new GetTransferConnectionInvitationsQuery
            {
                AccountId = _account.Id
            };
        }

        [Test]
        public async Task ThenShouldReturnGetTransferConnectionInvitationsResponse()
        {
            _response = await _handler.Handle(_query);

            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.TransferConnectionInvitations.Count(), Is.EqualTo(2));
            Assert.That(_response.TransferConnectionInvitations.ElementAt(0), Is.Not.Null);
            Assert.That(_response.TransferConnectionInvitations.ElementAt(0).Id, Is.EqualTo(_receivedTransferConnectionInvitation.Id));
            Assert.That(_response.TransferConnectionInvitations.ElementAt(1), Is.Not.Null);
            Assert.That(_response.TransferConnectionInvitations.ElementAt(1).Id, Is.EqualTo(_sentTransferConnectionInvitation.Id));
        }
    }
}