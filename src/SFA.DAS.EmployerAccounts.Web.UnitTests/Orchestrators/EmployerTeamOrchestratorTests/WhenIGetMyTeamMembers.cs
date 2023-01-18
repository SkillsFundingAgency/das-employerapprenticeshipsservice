using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.GetAccountTeamMembers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerTeamOrchestratorTests
{
    public class WhenIGetMyTeamMembers
    {
        private Mock<IMediator> _mediator;
        private Mock<IAccountApiClient> _accountApiClient;      

        private Mock<IMapper> _mapper;

        private EmployerTeamOrchestrator _orchestrator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<GetAccountTeamMembersQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(
                        new GetAccountTeamMembersResponse
                        {
                            TeamMembers = new List<TeamMember> {new TeamMember()}
                        });

            _accountApiClient = new Mock<IAccountApiClient>();           
            _mapper = new Mock<IMapper>();

            _orchestrator = new EmployerTeamOrchestrator(_mediator.Object, Mock.Of<ICurrentDateTime>(), _accountApiClient.Object, _mapper.Object, Mock.Of<EmployerAccountsConfiguration>());
        }
        
        [Test]
        public async Task ThenTheTeamMembersArePopulatedToTheResponse()
        {
            //Act
            var actual = await _orchestrator.GetTeamMembers("ABF45", "123");

            //Assert
            Assert.IsNotEmpty(actual.Data.TeamMembers);
        }
    }
}
