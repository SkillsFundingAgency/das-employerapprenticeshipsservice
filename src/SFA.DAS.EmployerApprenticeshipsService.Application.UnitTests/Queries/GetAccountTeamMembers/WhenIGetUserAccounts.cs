using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountTeamMembers;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Tests.Queries.GetAccountTeamMembers
{
    class WhenIGetUserAccountTeamMembers
    {
        private Mock<IAccountTeamRepository> _accountTeamMembersRepository;
        private GetAccountTeamMembersHandler _getAccountTeamMembersQueryHandler;
        private List<TeamMember> _teamMembers;
        private Account _account;
        private TeamMember _teamMember;
        private Mock<IValidator<GetAccountTeamMembersQuery>> _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<GetAccountTeamMembersQuery>>();
            _validator.Setup(x => x.Validate(It.IsAny<GetAccountTeamMembersQuery>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });
            _accountTeamMembersRepository = new Mock<IAccountTeamRepository>();
            _account = new Account {Name = "Test", Id = 1};
            _teamMember = new TeamMember
            {
                Id = 1,
                Email = "floyd@price.com",
                AccountId = _account.Id,
                Role = Role.Owner,
                UserRef = "kaka-kakah"
            };
            _teamMembers = new List<TeamMember> { _teamMember };
            _accountTeamMembersRepository.Setup(repository => repository.GetAccountTeamMembersForUserId(1, _teamMember.UserRef)).ReturnsAsync(new List<TeamMember>( _teamMembers));
            _getAccountTeamMembersQueryHandler = new GetAccountTeamMembersHandler(_validator.Object,_accountTeamMembersRepository.Object);

        }

        [Test]
        public async Task ThenTheUserRepositoryIsCalledToGetAllUsers()
        {
            //Act
             await _getAccountTeamMembersQueryHandler.Handle(new GetAccountTeamMembersQuery() {ExternalUserId = "kaka-kakah", Id = 1});

            //Assert
            _accountTeamMembersRepository.Verify(x => x.GetAccountTeamMembersForUserId(1, "kaka-kakah"), Times.Once);
        }

    }
}
