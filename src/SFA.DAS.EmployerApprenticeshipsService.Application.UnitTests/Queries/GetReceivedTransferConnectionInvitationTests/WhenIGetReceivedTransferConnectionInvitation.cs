using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Data;
using SFA.DAS.EAS.Application.Queries.GetReceivedTransferConnectionInvitation;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.TestCommon;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetReceivedTransferConnectionInvitationTests
{
    [TestFixture]
    public class WhenIGetReceivedTransferConnectionInvitation
    {
        private GetReceivedTransferConnectionInvitationQueryHandler _handler;
        private GetReceivedTransferConnectionInvitationQuery _query;
        private Mock<EmployerAccountDbContext> _db;
        private DbSetStub<TransferConnectionInvitation> _transferConnectionInvitationsDbSet;
        private List<TransferConnectionInvitation> _transferConnectionInvitations;
        private TransferConnectionInvitation _receivedTransferConnectionInvitation;
        private TransferConnectionInvitation _rejectedTransferConnectionInvitation;
        private Domain.Data.Entities.Account.Account _senderAccount;
        private Domain.Data.Entities.Account.Account _receiverAccount;

        [SetUp]
        public void Arrange()
        {
            _db = new Mock<EmployerAccountDbContext>();

            _senderAccount = new Domain.Data.Entities.Account.Account
            {
                HashedId = "ABC123",
                Id = 333333,
                Name = "Sender"
            };

            _receiverAccount = new Domain.Data.Entities.Account.Account
            {
                HashedId = "XYZ987",
                Id = 222222,
                Name = "Receiver"
            };

            _receivedTransferConnectionInvitation = new TransferConnectionInvitation
            {
                Id = 111111,
                SenderAccount = _senderAccount,
                ReceiverAccount = _receiverAccount,
                Status = TransferConnectionInvitationStatus.Pending
            };

            _rejectedTransferConnectionInvitation = new TransferConnectionInvitation
            {
                Id = 111111,
                SenderAccount = _senderAccount,
                ReceiverAccount = _receiverAccount,
                Status = TransferConnectionInvitationStatus.Rejected
            };

            _transferConnectionInvitations = new List<TransferConnectionInvitation> { _receivedTransferConnectionInvitation, _rejectedTransferConnectionInvitation };
            _transferConnectionInvitationsDbSet = new DbSetStub<TransferConnectionInvitation>(_transferConnectionInvitations);
            
            _db.Setup(d => d.TransferConnectionInvitations).Returns(_transferConnectionInvitationsDbSet);

            _handler = new GetReceivedTransferConnectionInvitationQueryHandler(_db.Object);

            _query = new GetReceivedTransferConnectionInvitationQuery
            {
                AccountId = _receiverAccount.Id,
                TransferConnectionInvitationId = _receivedTransferConnectionInvitation.Id
            };
        }

        [Test]
        public async Task ThenShouldReturnReceivedTransferConnectionInvitation()
        {
            var response = await _handler.Handle(_query);

            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.TypeOf<GetReceivedTransferConnectionInvitationResponse>());
            Assert.That(response.TransferConnectionInvitation, Is.Not.Null);
            Assert.That(response.TransferConnectionInvitation.Id, Is.EqualTo(_receivedTransferConnectionInvitation.Id));
        }

        [Test]
        public async Task ThenShouldReturnNullIfTransferConnectionInvitationIsNull()
        {
            _transferConnectionInvitations.Clear();

            var response = await _handler.Handle(_query);

            Assert.That(response, Is.Null);
        }
    }
}