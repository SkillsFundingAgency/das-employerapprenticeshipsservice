using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUserAccounts;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUserInvitations;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Orchestrators.HomeOrchestratorTests
{
    public class WhenGettingUserAccounts
    {
        private HomeOrchestrator _homeOrchestrator;
        private Mock<IMediator> _mediator;
        private Mock<IOwinWrapper> _owinWrapper;
        private readonly string ExpectedUserId = "12345ABC";

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUserAccountsQuery>())).ReturnsAsync(new GetUserAccountsQueryResponse {Accounts = new Accounts()});
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetNumberOfUserInvitationsQuery>())).ReturnsAsync(new GetNumberOfUserInvitationsResponse {NumberOfInvites = 2});

            _owinWrapper = new Mock<IOwinWrapper>();
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns(ExpectedUserId);

            _homeOrchestrator = new HomeOrchestrator(_mediator.Object, _owinWrapper.Object);
        }

        [Test]
        public async Task TheUserIdIsRetrievedFromTheOwinWrapperClaims()
        {
            //Act
            await _homeOrchestrator.GetUserAccounts();

            //Assert
            _owinWrapper.Verify(x=>x.GetClaimValue("sub"),Times.Once);
        }

        [Test]
        public async Task ThenNoQueriesAreCalledIfTheUserIdReturnsNothing()
        {
            //Arrange
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns(string.Empty);

            //Act
            await _homeOrchestrator.GetUserAccounts();

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetUserAccountsQuery>()), Times.Never);
        }

        [Test]
        public async Task ThenTheUserAccountsAreRetrievedFromTheQuery()
        {
            //Act
            await _homeOrchestrator.GetUserAccounts();

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<GetUserAccountsQuery>(c=>c.UserId.Equals(ExpectedUserId))), Times.Once);
        }

        [Test]
        public async Task ThenTheNumberOfInvitesIsReturnedForTheUser()
        {
            //Act
            await _homeOrchestrator.GetUserAccounts();

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetNumberOfUserInvitationsQuery>(c => c.UserId.Equals(ExpectedUserId))), Times.Once);
        }
    }
}
