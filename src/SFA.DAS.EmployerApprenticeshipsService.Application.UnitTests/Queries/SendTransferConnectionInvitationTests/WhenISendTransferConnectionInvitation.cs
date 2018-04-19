using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.EAS.Application.Mappings;
using SFA.DAS.EAS.Application.Queries.SendTransferConnectionInvitation;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.TestCommon;
using SFA.DAS.EAS.TestCommon.Builders;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.SendTransferConnectionInvitationTests
{
    [TestFixture]
    public class WhenISendTransferConnectionInvitation
    {
        private SendTransferConnectionInvitationQueryHandler _handler;
        private SendTransferConnectionInvitationQuery _query;
        private SendTransferConnectionInvitationResponse _response;
        private Mock<EmployerAccountDbContext> _db;
        private DbSetStub<Domain.Data.Entities.Account.Account> _accountsDbSet;
        private List<Domain.Data.Entities.Account.Account> _accounts;
        private DbSetStub<TransferConnectionInvitation> _transferConnectionInvitationsDbSet;
        private List<TransferConnectionInvitation> _transferConnectionInvitations;
        private Domain.Data.Entities.Account.Account _receiverAccount;
        private Domain.Data.Entities.Account.Account _senderAccount;
        private IConfigurationProvider _configurationProvider;
        private Mock<IPublicHashingService> _publicHashingService;

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
            _configurationProvider = new MapperConfiguration(c => c.AddProfile<AccountMappings>());
            _publicHashingService = new Mock<IPublicHashingService>();

            _db.Setup(d => d.Accounts).Returns(_accountsDbSet);
            _db.Setup(d => d.TransferConnectionInvitations).Returns(_transferConnectionInvitationsDbSet);
            _publicHashingService.Setup(h => h.DecodeValue(_receiverAccount.PublicHashedId)).Returns(_receiverAccount.Id);

            _handler = new SendTransferConnectionInvitationQueryHandler(_db.Object, _configurationProvider, _publicHashingService.Object);

            _query = new SendTransferConnectionInvitationQuery
            {
                AccountId = _senderAccount.Id,
                ReceiverAccountPublicHashedId = _receiverAccount.PublicHashedId
            };
        }

        [Test]
        public async Task ThenShouldReturnSendTransferConnectionInvitationResponse()
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

            var exception = Assert.ThrowsAsync<ValidationException<SendTransferConnectionInvitationQuery>>(async () => await _handler.Handle(_query));

            Assert.That(exception.PropertyName, Is.EqualTo(nameof(_query.ReceiverAccountPublicHashedId)));
            Assert.That(exception.Message, Is.EqualTo("You must enter a valid account ID"));
        }

        [Test]
        public void ThenShouldThrowValidationExceptionIfReceiverAccountHasSentAnInvitation()
        {
            _transferConnectionInvitations.Add(new TransferConnectionInvitationBuilder()
                .WithSenderAccount(_receiverAccount)
                .WithReceiverAccount(_senderAccount)
                .WithStatus(TransferConnectionInvitationStatus.Pending)
                .Build());

            var exception = Assert.ThrowsAsync<ValidationException<SendTransferConnectionInvitationQuery>>(async () => await _handler.Handle(_query));

            Assert.That(exception.PropertyName, Is.EqualTo(nameof(_query.ReceiverAccountPublicHashedId)));
            Assert.That(exception.Message, Is.EqualTo("You can't connect with this employer because they already have pending or accepted connection requests"));
        }

        [Test]
        public void ThenShouldThrowValidationExceptionIfPendingInvitationsExist()
        {
            _transferConnectionInvitations.Add(new TransferConnectionInvitationBuilder()
                .WithSenderAccount(_senderAccount)
                .WithReceiverAccount(_receiverAccount)
                .WithStatus(TransferConnectionInvitationStatus.Pending)
                .Build());

            var exception = Assert.ThrowsAsync<ValidationException<SendTransferConnectionInvitationQuery>>(async () => await _handler.Handle(_query));

            Assert.That(exception.PropertyName, Is.EqualTo(nameof(_query.ReceiverAccountPublicHashedId)));
            Assert.That(exception.Message, Is.EqualTo("You can't connect with this employer because they already have a pending or accepted connection request"));
        }

        [Test]
        public void ThenShouldThrowValidationExceptionIfApprovedInvitationsExist()
        {
            _transferConnectionInvitations.Add(new TransferConnectionInvitationBuilder()
                .WithSenderAccount(_senderAccount)
                .WithReceiverAccount(_receiverAccount)
                .WithStatus(TransferConnectionInvitationStatus.Approved)
                .Build());

            var exception = Assert.ThrowsAsync<ValidationException<SendTransferConnectionInvitationQuery>>(async () => await _handler.Handle(_query));

            Assert.That(exception.PropertyName, Is.EqualTo(nameof(_query.ReceiverAccountPublicHashedId)));
            Assert.That(exception.Message, Is.EqualTo("You can't connect with this employer because they already have a pending or accepted connection request"));
        }
    }
}