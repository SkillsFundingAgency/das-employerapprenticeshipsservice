using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Data;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitations;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.TestCommon;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetTransferConnectionInvitationsTests
{
    [TestFixture]
    public class WhenIGetTransferConnectionInvitations
    {
        private GetTransferConnectionInvitationsQueryHandler _handler;
        private GetTransferConnectionInvitationsQuery _query;
        private GetTransferConnectionInvitationsResponse _response;
        private Mock<EmployerAccountDbContext> _db;
        private DbSetStub<TransferConnectionInvitation> _transferConnectionInvitationsDbSet;
        private List<TransferConnectionInvitation> _transferConnectionInvitations;
        private TransferConnectionInvitation _sentTransferConnectionInvitation;
        private TransferConnectionInvitation _receivedTransferConnectionInvitation;
        private Domain.Data.Entities.Account.Account _account;

        [SetUp]
        public void Arrange()
        {
            _db = new Mock<EmployerAccountDbContext>();

            _account = new Domain.Data.Entities.Account.Account
            {
                Id = 333333,
                HashedId = "ABC123",
                Name = "Account"
            };

            _sentTransferConnectionInvitation = new TransferConnectionInvitation
            {
                Id = 222222,
                SenderAccount = _account,
                ReceiverAccount = new Domain.Data.Entities.Account.Account(),
                CreatedDate = DateTime.UtcNow
            };

            _receivedTransferConnectionInvitation = new TransferConnectionInvitation
            {
                Id = 111111,
                SenderAccount = new Domain.Data.Entities.Account.Account(),
                ReceiverAccount = _account,
                CreatedDate = DateTime.UtcNow.AddDays(-1)
            };

            _transferConnectionInvitations = new List<TransferConnectionInvitation>
            {
                _sentTransferConnectionInvitation,
                _receivedTransferConnectionInvitation,
                new TransferConnectionInvitation
                {
                    SenderAccount = new Domain.Data.Entities.Account.Account(),
                    ReceiverAccount = new Domain.Data.Entities.Account.Account(),
                    CreatedDate = DateTime.UtcNow.AddDays(-2)
                }
            };

            _transferConnectionInvitationsDbSet = new DbSetStub<TransferConnectionInvitation>(_transferConnectionInvitations);
            
            _db.Setup(d => d.TransferConnectionInvitations).Returns(_transferConnectionInvitationsDbSet);

            _handler = new GetTransferConnectionInvitationsQueryHandler(_db.Object);

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
            Assert.That(_response.AccountId, Is.EqualTo(_account.Id));
            Assert.That(_response.TransferConnectionInvitations.Count(), Is.EqualTo(2));
            Assert.That(_response.TransferConnectionInvitations.ElementAt(0), Is.Not.Null);
            Assert.That(_response.TransferConnectionInvitations.ElementAt(0).Id, Is.EqualTo(_receivedTransferConnectionInvitation.Id));
            Assert.That(_response.TransferConnectionInvitations.ElementAt(1), Is.Not.Null);
            Assert.That(_response.TransferConnectionInvitations.ElementAt(1).Id, Is.EqualTo(_sentTransferConnectionInvitation.Id));
        }
    }
}