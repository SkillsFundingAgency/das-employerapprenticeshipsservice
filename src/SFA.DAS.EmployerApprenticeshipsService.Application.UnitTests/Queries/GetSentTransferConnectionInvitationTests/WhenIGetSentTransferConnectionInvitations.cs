using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionInvitationQuery;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.TransferConnection;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetSentTransferConnectionInvitationTests
{
    public class WhenIGetSentTransferConnectionInvitations
    {
        private const string ExternalUserId = "ABCDEF";
        private const long UserId = 123456;
        private const long ReceiverAccountId = 111111;
        private const long SenderAccountId = 222222;
        private const long TransferConnectionInvitationId = 333333;

        private GetSentTransferConnectionInvitationQueryHandler _handler;
        private readonly GetSentTransferConnectionInvitationQuery _query = new GetSentTransferConnectionInvitationQuery { TransferConnectionInvitationId = TransferConnectionInvitationId };
        private readonly CurrentUser _currentUser = new CurrentUser { ExternalUserId = ExternalUserId };
        private readonly Mock<IEmployerAccountRepository> _employerAccountRepository = new Mock<IEmployerAccountRepository>();
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<ITransferConnectionInvitationRepository> _transferConnectionRepository;
        private TransferConnectionInvitation _transferConnectionInvitation;
        private readonly MembershipView _membershipView = new MembershipView { UserId = UserId };
        private readonly Domain.Data.Entities.Account.Account _receiverAccount = new Domain.Data.Entities.Account.Account { Id = ReceiverAccountId };

        [SetUp]
        public void SetUp()
        {
            _transferConnectionInvitation = new TransferConnectionInvitation
            {
                SenderAccountId = SenderAccountId,
                ReceiverAccountId = ReceiverAccountId
            };

            _membershipRepository = new Mock<IMembershipRepository>();
            _transferConnectionRepository = new Mock<ITransferConnectionInvitationRepository>();

            _transferConnectionRepository.Setup(r => r.GetSentTransferConnectionInvitation(TransferConnectionInvitationId)).ReturnsAsync(_transferConnectionInvitation);
            _membershipRepository.Setup(r => r.GetCaller(SenderAccountId, ExternalUserId)).ReturnsAsync(_membershipView);
            _employerAccountRepository.Setup(r => r.GetAccountById(ReceiverAccountId)).ReturnsAsync(_receiverAccount);

            _handler = new GetSentTransferConnectionInvitationQueryHandler(_currentUser, _employerAccountRepository.Object, _membershipRepository.Object, _transferConnectionRepository.Object);
        }

        [Test]
        public async Task ThenShouldGetTransferConnectionInvitation()
        {
            await _handler.Handle(_query);

            _transferConnectionRepository.Verify(r => r.GetSentTransferConnectionInvitation(TransferConnectionInvitationId), Times.Once);
        }

        [Test]
        public async Task ThenShouldGetUser()
        {
            await _handler.Handle(_query);

            _membershipRepository.Verify(r => r.GetCaller(SenderAccountId, ExternalUserId), Times.Once);
        }

        [Test]
        public async Task ThenShouldGetReceiversAccount()
        {
            await _handler.Handle(_query);

            _employerAccountRepository.Verify(r => r.GetAccountById(ReceiverAccountId), Times.Once);
        }

        [Test]
        public async Task ThenShouldReturnCreatedTransferConnectionInvitation()
        {
            var response = await _handler.Handle(_query);

            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.TypeOf<GetSentTransferConnectionInvitationResponse>());
            Assert.That(response.ReceiverAccount, Is.SameAs(_receiverAccount));
            Assert.That(response.TransferConnectionInvitation, Is.SameAs(_transferConnectionInvitation));
        }

        [Test]
        public async Task ThenShouldReturnNullIfTransferConnectionIsNull()
        {
            _transferConnectionRepository.Setup(r => r.GetSentTransferConnectionInvitation(TransferConnectionInvitationId)).ReturnsAsync(null);

            var model = await _handler.Handle(_query);

            Assert.That(model, Is.Null);
        }

        [Test]
        public void ThenShouldThrowUnauthorizedAccessExceptionIfUserIsNull()
        {
            _membershipRepository.Setup(r => r.GetCaller(SenderAccountId, ExternalUserId)).ReturnsAsync(null);

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _handler.Handle(_query));
        }
    }
}