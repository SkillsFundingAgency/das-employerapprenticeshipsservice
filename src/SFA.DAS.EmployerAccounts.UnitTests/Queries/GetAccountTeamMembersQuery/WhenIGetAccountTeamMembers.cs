using Moq;
using NUnit.Framework;
using SFA.DAS.Validation;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Queries.GetAccountTeamMembers;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.Authentication;
using MediatR;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using System.Security.Claims;
using SFA.DAS.EmployerAccounts.Configuration;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountTeamMembersQuery
{
    public class WhenIGetAccountTeamMembers : QueryBaseTest<GetAccountTeamMembersHandler, EmployerAccounts.Queries.GetAccountTeamMembers.GetAccountTeamMembersQuery, GetAccountTeamMembersResponse>
    {
        private Mock<IEmployerAccountTeamRepository> _employerAccountTeamRepository;
        private Mock<IAuthenticationService> _authenticationService;
        private Mock<IMediator> _mediator;
        private Mock<IMembershipRepository> _membershipRepository;
        private readonly string SupportConsoleUsers = "Tier1User,Tier2User";
        private EmployerAccountsConfiguration _config;
        public override EmployerAccounts.Queries.GetAccountTeamMembers.GetAccountTeamMembersQuery Query { get; set; }
        public override GetAccountTeamMembersHandler RequestHandler { get; set; }
        public override Mock<IValidator<EmployerAccounts.Queries.GetAccountTeamMembers.GetAccountTeamMembersQuery>> RequestValidator { get; set; }

        private const long AccountId = 1234;
        private const string ExpectedHashedAccountId = "MNBGBD";
        private const string ExpectedExternalUserId = "ABCGBD";
        private List<TeamMember> TeamMembers = new List<TeamMember>();

        [SetUp]
        public void Arrange()
        {
            SetUp();

            TeamMembers.Add(new TeamMember());

            _employerAccountTeamRepository = new Mock<IEmployerAccountTeamRepository>();
            _employerAccountTeamRepository
                .Setup(m => m.GetAccountTeamMembersForUserId(ExpectedHashedAccountId, ExpectedExternalUserId))
                .ReturnsAsync(TeamMembers);

            _authenticationService = new Mock<IAuthenticationService>();

            _authenticationService
                .Setup(m => m.HasClaim(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);                

            _mediator = new Mock<IMediator>();
            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository
                .Setup(m => m.GetCaller(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new MembershipView { AccountId = AccountId});

            _config = new EmployerAccountsConfiguration()
            {
                SupportConsoleUsers = SupportConsoleUsers
            };

            RequestHandler = new GetAccountTeamMembersHandler(
                RequestValidator.Object, 
                _employerAccountTeamRepository.Object,
                _authenticationService.Object,
                _mediator.Object,
                _membershipRepository.Object,
                _config);

            Query = new EmployerAccounts.Queries.GetAccountTeamMembers.GetAccountTeamMembersQuery();
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(new EmployerAccounts.Queries.GetAccountTeamMembers.GetAccountTeamMembersQuery
            {
                HashedAccountId = ExpectedHashedAccountId,
                ExternalUserId = ExpectedExternalUserId
            });

            //Assert
            _employerAccountTeamRepository.Verify(x => x.GetAccountTeamMembersForUserId(ExpectedHashedAccountId, ExpectedExternalUserId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var result = await RequestHandler.Handle(new EmployerAccounts.Queries.GetAccountTeamMembers.GetAccountTeamMembersQuery
            {
                HashedAccountId = ExpectedHashedAccountId,
                ExternalUserId = ExpectedExternalUserId
            });

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.TeamMembers);
        }

        [Test]
        [TestCase("Tier1User")]
        [TestCase("Tier2User")]
        public async Task ThenIfTheMessageIsValidAndTheCallerIsASupportUserThenTheMembershiprespositoryIsCalled(string role)
        {
            //Act
            _authenticationService
                .Setup(m => m.HasClaim(ClaimsIdentity.DefaultRoleClaimType, role))
                .Returns(true);

            await RequestHandler.Handle(new EmployerAccounts.Queries.GetAccountTeamMembers.GetAccountTeamMembersQuery
            {
                HashedAccountId = ExpectedHashedAccountId,
                ExternalUserId = ExpectedExternalUserId
            });

            //Assert
            _membershipRepository.Verify(x => x.GetCaller(ExpectedHashedAccountId, ExpectedExternalUserId), Times.Once);
        }

        [Test]
        [TestCase("Tier1User")]
        [TestCase("Tier2User")]
        public async Task ThenIfTheMessageIsValidAndTheCallerIsASupportUserThenTheAuditIsRaised(string role)
        {
            //Act
            _authenticationService
                .Setup(m => m.HasClaim(ClaimsIdentity.DefaultRoleClaimType, role))
                .Returns(true);

            await RequestHandler.Handle(new EmployerAccounts.Queries.GetAccountTeamMembers.GetAccountTeamMembersQuery
            {
                HashedAccountId = ExpectedHashedAccountId,
                ExternalUserId = ExpectedExternalUserId
            });

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c => 
            c.EasAuditMessage.Category.Equals("VIEW") &&
            c.EasAuditMessage.Description.Equals($"Account {AccountId} team members viewed")
            )), Times.Once);
        }

        [Test]
        public async Task ThenIfTheMessageIsValidAndTheCallerIsNotASupportUserThenTheAuditIsNotRaised()
        {
            //Act
            await RequestHandler.Handle(new EmployerAccounts.Queries.GetAccountTeamMembers.GetAccountTeamMembersQuery
            {
                HashedAccountId = ExpectedHashedAccountId,
                ExternalUserId = ExpectedExternalUserId
            });

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
            c.EasAuditMessage.Category.Equals("VIEW") &&
            c.EasAuditMessage.Description.Equals($"Account {AccountId} team members viewed")
            )), Times.Never);
        }
    }
}
