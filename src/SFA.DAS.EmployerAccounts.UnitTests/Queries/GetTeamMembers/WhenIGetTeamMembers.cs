using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.GetTeamMembers;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetTeamMembers
{
    public class WhenIGetTeamMembers : QueryBaseTest<GetTeamMembersRequestHandler, GetTeamMembersRequest, GetTeamMembersResponse>
    {
        public override GetTeamMembersRequest Query { get; set; }
        public override GetTeamMembersRequestHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetTeamMembersRequest>> RequestValidator { get; set; }

        private Mock<IEmployerAccountTeamRepository> _repository;
        private TeamMember _teamMember;
        private Mock<ILogger<GetTeamMembersRequestHandler>> _logger;

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _teamMember = new TeamMember();
            _logger = new Mock<ILogger<GetTeamMembersRequestHandler>>();

            _repository = new Mock<IEmployerAccountTeamRepository>();

            _repository.Setup(x => x.GetAccountTeamMembers(It.IsAny<long>()))
                .ReturnsAsync(new List<TeamMember> { _teamMember });
            
            RequestHandler = new GetTeamMembersRequestHandler(_repository.Object, RequestValidator.Object, _logger.Object);

            Query = new GetTeamMembersRequest(756);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            _repository.Verify(x => x.GetAccountTeamMembers(Query.AccountId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            Assert.Contains(_teamMember, (ICollection) response.TeamMembers);
        }
    }
}
