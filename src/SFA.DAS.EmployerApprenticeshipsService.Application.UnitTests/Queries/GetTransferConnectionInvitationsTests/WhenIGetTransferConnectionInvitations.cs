using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitations;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetTransferConnectionInvitationsTests
{
    public class WhenIGetTransferConnectionInvitations
    {
        private const string SenderHashedAccountId = "ABC123";
        private const string ExternalUserId = "ABCDEF";
        private const long UserId = 123456;
        private const long SenderAccountId = 222222;

        private GetTransferConnectionInvitationsQueryHandler _handler;
        private GetTransferConnectionInvitationsQuery _query;
        private GetTransferConnectionInvitationsResponse _response;
        private readonly CurrentUser _currentUser = new CurrentUser { ExternalUserId = ExternalUserId };
        private readonly Mock<IHashingService> _hashingService = new Mock<IHashingService>();
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<ITransferConnectionInvitationRepository> _transferConnectionRepository;
        private readonly MembershipView _membershipView = new MembershipView { UserId = UserId };
        private readonly IEnumerable<TransferConnectionInvitation> _sentTransferConnectionInvitations = new List<TransferConnectionInvitation>();

        [SetUp]
        public void SetUp()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _transferConnectionRepository = new Mock<ITransferConnectionInvitationRepository>();
            
            _hashingService.Setup(h => h.DecodeValue(SenderHashedAccountId)).Returns(SenderAccountId);
            _membershipRepository.Setup(r => r.GetCaller(SenderHashedAccountId, ExternalUserId)).ReturnsAsync(_membershipView);
            _transferConnectionRepository.Setup(r => r.GetSentTransferConnectionInvitations(SenderAccountId)).ReturnsAsync(_sentTransferConnectionInvitations);

            _handler = new GetTransferConnectionInvitationsQueryHandler(
                _currentUser,
                _hashingService.Object,
                _membershipRepository.Object,
                _transferConnectionRepository.Object
            );

            _query = new GetTransferConnectionInvitationsQuery
            {
                HashedAccountId = SenderHashedAccountId
            };
        }

        [Test]
        public async Task ThenShouldGetUser()
        {
            _response = await _handler.Handle(_query);

            _membershipRepository.Verify(r => r.GetCaller(SenderHashedAccountId, ExternalUserId), Times.Once);
        }

        [Test]
        public async Task ThenShouldGetTransferConnectionInvitations()
        {
            _response = await _handler.Handle(_query);

            _transferConnectionRepository.Verify(r => r.GetSentTransferConnectionInvitations(SenderAccountId), Times.Once);
        }

        [Test]
        public async Task ThenShouldReturnGetTransferConnectionInvitationsResponse()
        {
            _response = await _handler.Handle(_query);

            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.TransferConnectionInvitations, Is.SameAs(_sentTransferConnectionInvitations));
        }

        [Test]
        public void ThenShouldThrowUnauthorizedAccessExceptionIfUserIsNull()
        {
            _membershipRepository.Setup(r => r.GetCaller(SenderHashedAccountId, ExternalUserId)).ReturnsAsync(null);

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _handler.Handle(_query));
        }
    }
}