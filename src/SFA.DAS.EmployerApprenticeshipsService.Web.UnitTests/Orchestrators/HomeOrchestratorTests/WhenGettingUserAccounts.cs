using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetUserAccounts;
using SFA.DAS.EAS.Application.Queries.GetUserInvitations;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.HomeOrchestratorTests
{
    public class WhenGettingUserAccounts
    {
        private HomeOrchestrator _homeOrchestrator;
        private Mock<IMediator> _mediator;
        private readonly string ExpectedUserId = "12345ABC";

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUserAccountsQuery>())).ReturnsAsync(new GetUserAccountsQueryResponse {Accounts = new Accounts<Account>()});
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetNumberOfUserInvitationsQuery>())).ReturnsAsync(new GetNumberOfUserInvitationsResponse {NumberOfInvites = 2});
            

            _homeOrchestrator = new HomeOrchestrator(_mediator.Object);
        }
        
        [Test]
        public async Task ThenTheUserAccountsAreRetrievedFromTheQuery()
        {
            //Act
            await _homeOrchestrator.GetUserAccounts(ExpectedUserId);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<GetUserAccountsQuery>(c=>c.UserId.Equals(ExpectedUserId))), Times.Once);
        }

        [Test]
        public async Task ThenTheNumberOfInvitesIsReturnedForTheUser()
        {
            //Act
            await _homeOrchestrator.GetUserAccounts(ExpectedUserId);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetNumberOfUserInvitationsQuery>(c => c.UserId.Equals(ExpectedUserId))), Times.Once);
        }
    }
}
