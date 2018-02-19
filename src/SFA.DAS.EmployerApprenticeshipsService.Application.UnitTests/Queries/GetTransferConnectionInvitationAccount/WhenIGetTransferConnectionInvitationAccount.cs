using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAccount;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetTransferConnectionInvitationAccount
{
    public class WhenIGetTransferConnectionInvitationAccount
    {
        private const string SenderHashedAccountId = "ABC123";
        private const string ReceiverHashedAccountId = "XYZ987";
        private const string ExternalUserId = "ABCDEF";
        private const long UserId = 123456;
        private const long ReceiverAccountId = 111111;
        private const long SenderAccountId = 222222;

        private GetTransferConnectionInvitationAccountQueryHandler _handler;
        private GetTransferConnectionInvitationAccountQuery _query;
        private GetTransferConnectionInvitationAccountResponse _response;
        private readonly CurrentUser _currentUser = new CurrentUser { ExternalUserId = ExternalUserId };
        private readonly Mock<IHashingService> _hashingService = new Mock<IHashingService>();
        private Mock<IEmployerAccountRepository> _employerAccountRepository;
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<ITransferConnectionInvitationRepository> _transferConnectionRepository;
        private readonly MembershipView _membershipView = new MembershipView { UserId = UserId };
        private readonly Domain.Data.Entities.Account.Account _receiverAccount = new Domain.Data.Entities.Account.Account { Id = ReceiverAccountId };
        private readonly Domain.Data.Entities.Account.Account _senderAccount = new Domain.Data.Entities.Account.Account { Id = SenderAccountId };

        [SetUp]
        public void SetUp()
        {
            _employerAccountRepository = new Mock<IEmployerAccountRepository>();
            _membershipRepository = new Mock<IMembershipRepository>();
            _transferConnectionRepository = new Mock<ITransferConnectionInvitationRepository>();
            
            _hashingService.Setup(h => h.DecodeValue(SenderHashedAccountId)).Returns(SenderAccountId);
            _membershipRepository.Setup(r => r.GetCaller(SenderHashedAccountId, ExternalUserId)).ReturnsAsync(_membershipView);
            _employerAccountRepository.Setup(r => r.GetAccountByPublicHashedId(ReceiverHashedAccountId)).ReturnsAsync(_receiverAccount);
            _employerAccountRepository.Setup(r => r.GetAccountById(SenderAccountId)).ReturnsAsync(_senderAccount);

            _transferConnectionRepository.Setup(r => r.GetTransferConnectionInvitations(SenderAccountId, ReceiverAccountId))
                .ReturnsAsync(new List<TransferConnectionInvitation>());

            _handler = new GetTransferConnectionInvitationAccountQueryHandler(
                _currentUser,
                _employerAccountRepository.Object,
                _hashingService.Object,
                _membershipRepository.Object,
                _transferConnectionRepository.Object
            );

            _query = new GetTransferConnectionInvitationAccountQuery
            {
                SenderAccountHashedId = SenderHashedAccountId,
                ReceiverAccountPublicHashedId = ReceiverHashedAccountId
            };
        }

        [Test]
        public async Task ThenShouldGetUser()
        {
            _response = await _handler.Handle(_query);

            _membershipRepository.Verify(r => r.GetCaller(SenderHashedAccountId, ExternalUserId), Times.Once);
        }

        [Test]
        public async Task ThenShouldGetReceiversAccount()
        {
            _response = await _handler.Handle(_query);

            _employerAccountRepository.Verify(r => r.GetAccountByPublicHashedId(ReceiverHashedAccountId), Times.Once);
        }

        [Test]
        public async Task ThenShouldGetSendersAccount()
        {
            _response = await _handler.Handle(_query);

            _employerAccountRepository.Verify(r => r.GetAccountById(SenderAccountId), Times.Once);
        }

        [Test]
        public async Task ThenShouldGetTransferConnectionInvitations()
        {
            _response = await _handler.Handle(_query);

            _transferConnectionRepository.Verify(r => r.GetTransferConnectionInvitations(SenderAccountId, ReceiverAccountId), Times.Once);
        }

        [Test]
        public async Task ThenShouldReturnGetTransferConnectionInvitationAccountResponse()
        {
            _response = await _handler.Handle(_query);

            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.ReceiverAccount, Is.SameAs(_receiverAccount));
            Assert.That(_response.SenderAccount, Is.SameAs(_senderAccount));
        }

        [Test]
        public void ThenShouldThrowUnauthorizedAccessExceptionIfUserIsNull()
        {
            _membershipRepository.Setup(r => r.GetCaller(SenderHashedAccountId, ExternalUserId)).ReturnsAsync(null);

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _handler.Handle(_query));
        }

        [Test]
        public void ThenShouldThrowValidationExceptionIfReceiverAccountIsNull()
        {
            _employerAccountRepository.Setup(r => r.GetAccountByPublicHashedId(ReceiverHashedAccountId)).ReturnsAsync(null);

            var exception = Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(_query));

            Assert.That(exception.PropertyName, Is.EqualTo(nameof(_query.ReceiverAccountPublicHashedId)));
            Assert.That(exception.ErrorMessage, Is.EqualTo("You must enter a valid account ID"));
        }

        [Test]
        public void ThenShouldThrowValidationExceptionIfInvitationsAlreadySent()
        {
            _transferConnectionRepository.Setup(r => r.GetTransferConnectionInvitations(SenderAccountId, ReceiverAccountId))
                .ReturnsAsync(new List<TransferConnectionInvitation>
                {
                    new TransferConnectionInvitation
                    {
                        Status = TransferConnectionInvitationStatus.Sent
                    }
                });

            var exception = Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(_query));

            Assert.That(exception.PropertyName, Is.EqualTo(nameof(_query.ReceiverAccountPublicHashedId)));
            Assert.That(exception.ErrorMessage, Is.EqualTo("You've already sent a connection request to this employer"));
        }
    }
}