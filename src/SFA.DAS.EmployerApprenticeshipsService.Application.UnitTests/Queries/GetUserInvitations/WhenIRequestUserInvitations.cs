using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetUserInvitations;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetUserInvitations
{
    [TestFixture]
    public class WhenIRequestUserInvitations
    {
        private Mock<IInvitationRepository> _invitationRepository;
        private GetUserInvitationsQueryHandler _handler;
        private Mock<IHashingService> _hashingService;

        [SetUp]
        public void Setup()
        {
            _invitationRepository = new Mock<IInvitationRepository>();
            _invitationRepository.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(new List<InvitationView> {new InvitationView {Id = 3456776} });
            _invitationRepository.Setup(x => x.Get("user1")).ReturnsAsync(new List<InvitationView> { });

            _hashingService = new Mock<IHashingService>();
            _handler = new GetUserInvitationsQueryHandler(_invitationRepository.Object, _hashingService.Object);
        }

        [Test]
        public async Task ThenTheInvitationIdIsHashedByTheHashingService()
        {
            //Act
            await _handler.Handle(new GetUserInvitationsRequest());

            //Assert
            _hashingService.Verify(x=>x.HashValue(It.IsAny<long>()));
        }

        [Test]
        public void ThenIGetAnEmptyListInTheResponseWhenThereAreNoInvitations()
        {
            var request = new GetUserInvitationsRequest
            {
                UserId = "user1"
            };
            
            var response = _handler.Handle(request);

            Assert.That(response.Result.Invitations, Is.EqualTo(new List<InvitationView>()));
        }

        [Test]
        public void ThenIGetAnInvitation()
        {
            const string userId = "user1";
            const long invitationId = 1;

            var request = new GetUserInvitationsRequest
            {
                UserId = userId
            };

            var invitations = new List<InvitationView>
            {
                new InvitationView
                {
                    Id = invitationId,
                    AccountName = "Test",
                    Status = InvitationStatus.Pending,
                    ExpiryDate = new DateTime(2016, 12, 1)
                }
            };

            _invitationRepository.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(invitations);

            var response = _handler.Handle(request);

            Assert.That(response.Result.Invitations.Count, Is.EqualTo(1));

            var item = response.Result.Invitations.FirstOrDefault(x => x.Id == invitationId);

            Assert.That(item, Is.Not.Null);
        }

        [Test]
        public void ThenIGetSomeInvitations()
        {
            const string userId = "user1";
            const long invitationId = 1;

            var request = new GetUserInvitationsRequest
            {
                UserId = userId
            };

            var invitations = new List<InvitationView>
            {
                new InvitationView
                {
                    Id = invitationId,
                    AccountName = "Test",
                    Status = InvitationStatus.Pending,
                    ExpiryDate = new DateTime(2016, 12, 1)
                },
                new InvitationView
                {
                    Id = invitationId + 1,
                    AccountName = "Not Test",
                    Status = InvitationStatus.Pending,
                    ExpiryDate = new DateTime(2016, 12, 7)
                }
            };

            _invitationRepository.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(invitations);

            var response = _handler.Handle(request);

            Assert.That(response.Result.Invitations.Count, Is.EqualTo(2));

            var item = response.Result.Invitations.FirstOrDefault(x => x.Id == invitationId);

            Assert.That(item, Is.Not.Null);

            item = response.Result.Invitations.FirstOrDefault(x => x.Id != invitationId);

            Assert.That(item, Is.Not.Null);
        }
    }
}