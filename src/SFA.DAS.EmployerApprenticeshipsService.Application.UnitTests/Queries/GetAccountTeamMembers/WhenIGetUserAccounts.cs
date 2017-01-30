using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetAccountTeamMembers;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountTeamMembers
{
    class WhenIGetUserAccountTeamMembers : QueryBaseTest<GetAccountTeamMembersHandler, GetAccountTeamMembersQuery, GetAccountTeamMembersResponse>
    {
        private Mock<IAccountTeamRepository> _accountTeamMembersRepository;
        private List<TeamMember> _teamMembers;
        private Account _account;
        private TeamMember _teamMember;
        public override GetAccountTeamMembersQuery Query { get; set; }
        public override GetAccountTeamMembersHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountTeamMembersQuery>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            SetUp();
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
            _accountTeamMembersRepository.Setup(repository => repository.GetAccountTeamMembersForUserId("1", _teamMember.UserRef)).ReturnsAsync(new List<TeamMember>( _teamMembers));
            RequestHandler = new GetAccountTeamMembersHandler(RequestValidator.Object,_accountTeamMembersRepository.Object);
            Query = new GetAccountTeamMembersQuery {ExternalUserId = "kaka-kakah", HashedAccountId = "1"};
        }
        
        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _accountTeamMembersRepository.Verify(x => x.GetAccountTeamMembersForUserId("1", "kaka-kakah"), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotEmpty(actual.TeamMembers);
            Assert.AreEqual(1, actual.TeamMembers.Count);
        }

        [Test]
        public void ThenAnUnauthorizedExceptionsIsThrownWhenTheValidationResultIsMarkedAsNotAuthorized()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetAccountTeamMembersQuery>()))
                .ReturnsAsync(new ValidationResult
                {
                    IsUnauthorized = true,
                    ValidationDictionary = new Dictionary<string, string> {{"", ""}}
                });

            //Act
            Assert.ThrowsAsync<UnauthorizedAccessException>(async ()=>  await RequestHandler.Handle(Query));
        }
    }
}
