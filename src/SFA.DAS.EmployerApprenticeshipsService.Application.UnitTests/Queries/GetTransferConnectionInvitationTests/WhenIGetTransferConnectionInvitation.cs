using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Data;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitation;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetTransferConnectionInvitationTests
{
    [TestFixture]
    public class WhenIGetTransferConnectionInvitation
    {
        private GetTransferConnectionInvitationQueryHandler _handler;
        private GetTransferConnectionInvitationQuery _query;
        private Mock<EmployerAccountDbContext> _db;
        private Mock<IHashingService> _hashingService;
        private DbSetStub<TransferConnectionInvitation> _transferConnectionInvitationsDbSet;
        private List<TransferConnectionInvitation> _transferConnectionInvitations;
        private TransferConnectionInvitation _transferConnectionInvitation;
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

            _transferConnectionInvitation = new TransferConnectionInvitation
            {
                Id = 111111,
                SenderAccount = _senderAccount,
                ReceiverAccount = _receiverAccount
            };

            _transferConnectionInvitations = new List<TransferConnectionInvitation> { _transferConnectionInvitation, new TransferConnectionInvitation() };
            _transferConnectionInvitationsDbSet = new DbSetStub<TransferConnectionInvitation>(_transferConnectionInvitations);
            
            _hashingService.Setup(s => s.DecodeValue(_senderAccount.HashedId)).Returns(_senderAccount.Id);
            _db.Setup(d => d.TransferConnectionInvitations).Returns(_transferConnectionInvitationsDbSet);

            _handler = new GetTransferConnectionInvitationQueryHandler(_db.Object, _hashingService.Object);

            _query = new GetTransferConnectionInvitationQuery
            {
                AccountHashedId = _senderAccount.HashedId,
                TransferConnectionInvitationId = _transferConnectionInvitation.Id
            };
        }

        [Test]
        public async Task ThenShouldReturnTransferConnectionInvitation()
        {
            var response = await _handler.Handle(_query);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.AccountId, Is.EqualTo(_senderAccount.Id));
            Assert.That(response.TransferConnectionInvitation, Is.Not.Null);
            Assert.That(response.TransferConnectionInvitation.Id, Is.EqualTo(_transferConnectionInvitation.Id));
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