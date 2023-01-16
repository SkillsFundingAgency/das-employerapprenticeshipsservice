using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.GetTeamMembers;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetTeamMembers
{
    public class WhenIGetTeamMembers : QueryBaseTest<GetTeamMembersRequestHandler, GetTeamMembersRequest, GetTeamMembersResponse>
    {
        public override GetTeamMembersRequest Query { get; set; }
        public override GetTeamMembersRequestHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetTeamMembersRequest>> RequestValidator { get; set; }

        private Mock<IEmployerAccountTeamRepository> _repository;
        private TeamMember _teamMember;
        private Mock<ILog> _logger;

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _teamMember = new TeamMember();
            _logger = new Mock<ILog>();

            _repository = new Mock<IEmployerAccountTeamRepository>();

            _repository.Setup(x => x.GetAccountTeamMembers(It.IsAny<string>()))
                .ReturnsAsync(new List<TeamMember> { _teamMember });
            
            RequestHandler = new GetTeamMembersRequestHandler(_repository.Object, RequestValidator.Object, _logger.Object);

            Query = new GetTeamMembersRequest
            {
                HashedAccountId = "123ABC"
            };
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            _repository.Verify(x => x.GetAccountTeamMembers(Query.HashedAccountId), Times.Once);
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
