using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Data;
using SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionInvitation;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.TestCommon;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetSentTransferConnectionInvitationTests
{
    [TestFixture]
    public class WhenIGetSentTransferConnectionInvitation
    {
        private GetSentTransferConnectionInvitationQueryHandler _handler;
        private GetSentTransferConnectionInvitationQuery _query;
        private Mock<EmployerAccountDbContext> _db;
        private DbSetStub<TransferConnectionInvitation> _transferConnectionInvitationsDbSet;
        private List<TransferConnectionInvitation> _transferConnectionInvitations;
        private TransferConnectionInvitation _sentTransferConnectionInvitation;
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
                Id = 444444,
                Name = "Sender"
            };

            _receiverAccount = new Domain.Data.Entities.Account.Account
            {
                HashedId = "XYZ987",
                Id = 333333,
                Name = "Receiver"
            };

            _sentTransferConnectionInvitation = new TransferConnectionInvitation
            {
                Id = 222222,
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

            _transferConnectionInvitations = new List<TransferConnectionInvitation> { _sentTransferConnectionInvitation, _rejectedTransferConnectionInvitation };
            _transferConnectionInvitationsDbSet = new DbSetStub<TransferConnectionInvitation>(_transferConnectionInvitations);
            
            _db.Setup(d => d.TransferConnectionInvitations).Returns(_transferConnectionInvitationsDbSet);

            _handler = new GetSentTransferConnectionInvitationQueryHandler(_db.Object);

            _query = new GetSentTransferConnectionInvitationQuery
            {
                AccountId = _senderAccount.Id,
                TransferConnectionInvitationId = _sentTransferConnectionInvitation.Id
            };
        }

        [Test]
        public async Task ThenShouldReturnSentTransferConnectionInvitation()
        {
            var response = await _handler.Handle(_query);

            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.TypeOf<GetSentTransferConnectionInvitationResponse>());
            Assert.That(response.TransferConnectionInvitation, Is.Not.Null);
            Assert.That(response.TransferConnectionInvitation.Id, Is.EqualTo(_sentTransferConnectionInvitation.Id));
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