using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.GetTeamMembers;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Orchestrators.AccountsOrchestratorTests
{
    internal class WhenIGetAnAccountsUsers
    {
        private AccountsOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILogger<AccountsOrchestrator>> _log;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _log = new Mock<ILogger<AccountsOrchestrator>>();

            _orchestrator = new AccountsOrchestrator(_mediator.Object, _log.Object, Mock.Of<IMapper>(), Mock.Of<IHashingService>());

            _mediator
                .Setup(x => x.Send(It.IsAny<GetTeamMembersRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetTeamMembersResponse { TeamMembers = new List<TeamMember>() })
                .Verifiable("Get account was not called");
        }

        [Test]
        public async Task TheARequestToGetAccountUsersShouldBeMade()
        {
            //Arrange
            const string hashedAgreementId = "ABC123";

            //Act
            await _orchestrator.GetAccountTeamMembers(hashedAgreementId);

            //Assert
            _mediator.VerifyAll();
        }
    }
}
