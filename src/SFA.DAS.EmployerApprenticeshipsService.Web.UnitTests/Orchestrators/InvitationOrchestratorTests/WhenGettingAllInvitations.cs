using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Queries.GetUserAccounts;
using SFA.DAS.EAS.Application.Queries.GetUserInvitations;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.InvitationOrchestratorTests
{
    public class WhenGettingAllInvitations
    {
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private InvitationOrchestrator _invitationOrchestrator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUserInvitationsRequest>())).ReturnsAsync(new GetUserInvitationsResponse {Invitations = new List<InvitationView> {new InvitationView()} });
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUserAccountsQuery>())).ReturnsAsync(new GetUserAccountsQueryResponse
                {
                    Accounts = new Accounts<Account> { AccountList = new List<Account>()}
                });

            _logger = new Mock<ILogger>();

            _invitationOrchestrator = new InvitationOrchestrator(_mediator.Object, _logger.Object);
        }

        [Test]
        public async Task ThenTheMediatorIsCalledToRetrieveTheInvitations()
        {
            //Arrange
            var userId = "123abc";

            //Act
            var actual = await _invitationOrchestrator.GetAllInvitationsForUser(userId);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<GetUserInvitationsRequest>(c=>c.UserId.Equals(userId))), Times.Once);
            Assert.IsAssignableFrom<UserInvitationsViewModel>(actual.Data);
        }

        [TestCase(1, "Today")]
        [TestCase(2, "1 day")]
        [TestCase(4, "3 days")]
        public async Task ThenTheExpiresInDayIsCorrectlyCalculated(int days, string expectedValue)
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUserInvitationsRequest>())).ReturnsAsync(new GetUserInvitationsResponse
            {
                Invitations = new List<InvitationView> {new InvitationView
            {
                ExpiryDate = DateTime.UtcNow.AddDays(days).Date
            } }
            });

            //Act
            var actual = await _invitationOrchestrator.GetAllInvitationsForUser("123abc");

            //Assert
            Assert.AreEqual(expectedValue, actual.Data.Invitations.FirstOrDefault().ExpiryDays());
        }

        [Test]
        public async Task ThenNullIsReturnedWhenAnInvalidRequestExceptionIsThrown()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUserInvitationsRequest>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

            //act
            var actual = await _invitationOrchestrator.GetAllInvitationsForUser("test");

            //Assert
            Assert.IsNull(actual.Data);
            _logger.Verify(x=>x.Info(It.IsAny<InvalidRequestException>()), Times.Once);
        }

        [Test]
        public async Task ThenICheckToSeeWhatAccountsIHaveAccessTo()
        {
            //Arrange
            var expectedUserId = "123FVD";

            //Act
            await _invitationOrchestrator.GetAllInvitationsForUser(expectedUserId);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetUserAccountsQuery>(c => c.UserRef.Equals(expectedUserId))), Times.Once);
        }
    }
}
