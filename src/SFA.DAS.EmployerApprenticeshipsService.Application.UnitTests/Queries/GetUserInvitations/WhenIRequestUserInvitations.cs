using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUserInvitations;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Tests.Queries.GetUserInvitations
{
    [TestFixture]
    public class WhenIRequestUserInvitations
    {
        private Mock<IInvitationRepository> _invitationRepository;
        private GetUserInvitationsQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            _invitationRepository = new Mock<IInvitationRepository>();
            _handler = new GetUserInvitationsQueryHandler(_invitationRepository.Object);
        }

        [Test]
        public void ThenIGetNoInvitations()
        {
            var request = new GetUserInvitationsRequest
            {
                UserId = "user1"
            };

            _invitationRepository.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(new List<InvitationView>());

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