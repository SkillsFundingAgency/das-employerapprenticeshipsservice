using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Data;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAccount;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.TestCommon;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetTransferConnectionInvitationAccountTests
{
    [TestFixture]
    public class WhenIGetTransferConnectionInvitationAccount
    {
        private GetTransferConnectionInvitationAccountQueryHandler _handler;
        private GetTransferConnectionInvitationAccountQuery _query;
        private GetTransferConnectionInvitationAccountResponse _response;
        private Mock<EmployerAccountDbContext> _db;
        private DbSetStub<Domain.Data.Entities.Account.Account> _accountsDbSet;
        private List<Domain.Data.Entities.Account.Account> _accounts;
        private DbSetStub<TransferConnectionInvitation> _transferConnectionInvitationsDbSet;
        private List<TransferConnectionInvitation> _transferConnectionInvitations;
        private Domain.Data.Entities.Account.Account _receiverAccount;
        private Domain.Data.Entities.Account.Account _senderAccount;

        [SetUp]
        public void Arrange()
        {
            _db = new Mock<EmployerAccountDbContext>();

            _receiverAccount = new Domain.Data.Entities.Account.Account
            {
                Id = 111111,
                PublicHashedId = "ABC123"
            };

            _senderAccount = new Domain.Data.Entities.Account.Account
            {
                Id = 222222,
                PublicHashedId = "XYZ987"
            };

            _accounts = new List<Domain.Data.Entities.Account.Account>{ _receiverAccount, _senderAccount };
            _accountsDbSet = new DbSetStub<Domain.Data.Entities.Account.Account>(_accounts);
            _transferConnectionInvitations = new List<TransferConnectionInvitation>();
            _transferConnectionInvitationsDbSet = new DbSetStub<TransferConnectionInvitation>(_transferConnectionInvitations);
            
            _db.Setup(d => d.Accounts).Returns(_accountsDbSet);
            _db.Setup(d => d.TransferConnectionInvitations).Returns(_transferConnectionInvitationsDbSet);

            _handler = new GetTransferConnectionInvitationAccountQueryHandler(_db.Object);

            _query = new GetTransferConnectionInvitationAccountQuery
            {
                AccountId = _senderAccount.Id,
                ReceiverAccountPublicHashedId = _receiverAccount.PublicHashedId
            };
        }

        [Test]
        public async Task ThenShouldReturnGetTransferConnectionInvitationAccountResponse()
        {
            _response = await _handler.Handle(_query);

            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.ReceiverAccount, Is.Not.Null);
            Assert.That(_response.ReceiverAccount.Id, Is.EqualTo(_receiverAccount.Id));
        }

        [Test]
        public void ThenShouldThrowValidationExceptionIfReceiverAccountIsNull()
        {
            _accounts.Remove(_receiverAccount);

            var exception = Assert.ThrowsAsync<ValidationException<GetTransferConnectionInvitationAccountQuery>>(async () => await _handler.Handle(_query));

            Assert.That(exception.PropertyName, Is.EqualTo(nameof(_query.ReceiverAccountPublicHashedId)));
            Assert.That(exception.Message, Is.EqualTo("You must enter a valid account ID"));
        }

        [Test]
        public void ThenShouldThrowValidationExceptionIfInvitationsAlreadySent()
        {
            _transferConnectionInvitations.Add(new TransferConnectionInvitation
            {
                SenderAccount = _senderAccount,
                ReceiverAccount = _receiverAccount,
                Status = TransferConnectionInvitationStatus.Pending
            });

            var exception = Assert.ThrowsAsync<ValidationException<GetTransferConnectionInvitationAccountQuery>>(async () => await _handler.Handle(_query));

            Assert.That(exception.PropertyName, Is.EqualTo(nameof(_query.ReceiverAccountPublicHashedId)));
            Assert.That(exception.Message, Is.EqualTo("You've already sent a connection request to this employer"));
        }

        [Test]
        public void ThenShouldThrowValidationExceptionIfInvitationsAlreadyApproved()
        {
            _transferConnectionInvitations.Add(new TransferConnectionInvitation
            {
                SenderAccount = _senderAccount,
                ReceiverAccount = _receiverAccount,
                Status = TransferConnectionInvitationStatus.Approved
            });

            var exception = Assert.ThrowsAsync<ValidationException<GetTransferConnectionInvitationAccountQuery>>(async () => await _handler.Handle(_query));

            Assert.That(exception.PropertyName, Is.EqualTo(nameof(_query.ReceiverAccountPublicHashedId)));
            Assert.That(exception.Message, Is.EqualTo("You're already connected with this employer"));
        }
    }
}