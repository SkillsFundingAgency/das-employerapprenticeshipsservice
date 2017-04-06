using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetTeamMembers;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetTeamMembers
{
    public class WhenIGetTeamMembers : QueryBaseTest<GetTeamMembersRequestHandler, GetTeamMembersRequest, GetTeamMembersResponse>
    {
        public override GetTeamMembersRequest Query { get; set; }
        public override GetTeamMembersRequestHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetTeamMembersRequest>> RequestValidator { get; set; }

        private Mock<IAccountTeamRepository> _repository;
        private TeamMember _teamMember;
        private Mock<ILogger> _logger;

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _teamMember = new TeamMember();
            _logger = new Mock<ILogger>();

            _repository = new Mock<IAccountTeamRepository>();

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
            await RequestHandler.Handle(Query);

            //Assert
            _repository.Verify(x => x.GetAccountTeamMembers(Query.HashedAccountId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await RequestHandler.Handle(Query);

            //Assert
            Assert.Contains(_teamMember, (ICollection)response.TeamMembers);
        }
    }
}
