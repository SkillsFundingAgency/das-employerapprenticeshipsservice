using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetCreatedTransferConnection;
using SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionQuery;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.TransferConnection;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetSentTransferConnectionTests
{
    public class WhenIGetSentTransferConnections
    {
        private const string ExternalUserId = "ABCDEF";
        private const long UserId = 123456;
        private const long ReceiverAccountId = 111111;
        private const long SenderAccountId = 222222;
        private const long TransferConnectionId = 333333;

        private GetSentTransferConnectionQueryHandler _handler;
        private readonly GetSentTransferConnectionQuery _query = new GetSentTransferConnectionQuery { TransferConnectionId = TransferConnectionId };
        private readonly CurrentUser _currentUser = new CurrentUser { ExternalUserId = ExternalUserId };
        private readonly Mock<IEmployerAccountRepository> _employerAccountRepository = new Mock<IEmployerAccountRepository>();
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<ITransferConnectionRepository> _transferConnectionRepository;
        private TransferConnection _transferConnection;
        private readonly MembershipView _membershipView = new MembershipView { UserId = UserId };
        private readonly Domain.Data.Entities.Account.Account _receiverAccount = new Domain.Data.Entities.Account.Account { Id = ReceiverAccountId };

        [SetUp]
        public void SetUp()
        {
            _transferConnection = new TransferConnection
            {
                SenderAccountId = SenderAccountId,
                ReceiverAccountId = ReceiverAccountId
            };

            _membershipRepository = new Mock<IMembershipRepository>();
            _transferConnectionRepository = new Mock<ITransferConnectionRepository>();

            _transferConnectionRepository.Setup(r => r.GetSentTransferConnection(TransferConnectionId)).ReturnsAsync(_transferConnection);
            _membershipRepository.Setup(r => r.GetCaller(SenderAccountId, ExternalUserId)).ReturnsAsync(_membershipView);
            _employerAccountRepository.Setup(r => r.GetAccountById(ReceiverAccountId)).ReturnsAsync(_receiverAccount);

            _handler = new GetSentTransferConnectionQueryHandler(_currentUser, _employerAccountRepository.Object, _membershipRepository.Object, _transferConnectionRepository.Object);
        }

        [Test]
        public async Task ThenShouldGetTransferConnection()
        {
            await _handler.Handle(_query);

            _transferConnectionRepository.Verify(r => r.GetSentTransferConnection(TransferConnectionId), Times.Once);
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
        public async Task ThenShouldReturnCreatedTransferConnections()
        {
            var model = await _handler.Handle(_query);

            Assert.That(model, Is.Not.Null);
            Assert.That(model, Is.TypeOf<SentTransferConnectionViewModel>());
            Assert.That(model.ReceiverAccount, Is.SameAs(_receiverAccount));
            Assert.That(model.TransferConnection, Is.SameAs(_transferConnection));
        }

        [Test]
        public async Task ThenShouldReturnNullIfTransferConnectionIsNull()
        {
            _transferConnectionRepository.Setup(r => r.GetSentTransferConnection(TransferConnectionId)).ReturnsAsync(null);

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