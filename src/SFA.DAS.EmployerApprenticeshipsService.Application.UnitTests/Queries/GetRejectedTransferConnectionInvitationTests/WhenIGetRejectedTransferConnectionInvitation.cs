using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Data;
using SFA.DAS.EAS.Application.Queries.GetRejectedTransferConnectionInvitation;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetRejectedTransferConnectionInvitationTests
{
    [TestFixture]
    public class WhenIGetRejectedTransferConnectionInvitation
    {
        private GetRejectedTransferConnectionInvitationQueryHandler _handler;
        private GetRejectedTransferConnectionInvitationQuery _query;
        private Mock<EmployerAccountDbContext> _db;
        private Mock<IHashingService> _hashingService;
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
            _hashingService = new Mock<IHashingService>();

            _senderAccount = new Domain.Data.Entities.Account.Account
            {
                Id = 333333,
                HashedId = "ABC123",
                Name = "Sender"
            };

            _receiverAccount = new Domain.Data.Entities.Account.Account
            {
                Id = 222222,
                HashedId = "XYZ987",
                Name = "Receiver"
            };

            _sentTransferConnectionInvitation = new TransferConnectionInvitation
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

            _transferConnectionInvitations = new List<TransferConnectionInvitation> { _sentTransferConnectionInvitation, _rejectedTransferConnectionInvitation };
            _transferConnectionInvitationsDbSet = new DbSetStub<TransferConnectionInvitation>(_transferConnectionInvitations);
            
            _hashingService.Setup(h => h.DecodeValue(_senderAccount.HashedId)).Returns(_receiverAccount.Id);
            _db.Setup(d => d.TransferConnectionInvitations).Returns(_transferConnectionInvitationsDbSet);

            _handler = new GetRejectedTransferConnectionInvitationQueryHandler(_db.Object, _hashingService.Object);

            _query = new GetRejectedTransferConnectionInvitationQuery
            {
                AccountHashedId = _senderAccount.HashedId,
                TransferConnectionInvitationId = _rejectedTransferConnectionInvitation.Id
            };
        }

        [Test]
        public async Task ThenShouldReturnRejectedTransferConnectionInvitation()
        {
            var response = await _handler.Handle(_query);

            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.TypeOf<GetRejectedTransferConnectionInvitationResponse>());
            Assert.That(response.TransferConnectionInvitation, Is.Not.Null);
            Assert.That(response.TransferConnectionInvitation.Id, Is.EqualTo(_rejectedTransferConnectionInvitation.Id));
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