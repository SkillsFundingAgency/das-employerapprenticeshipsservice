using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EAS.Application.Queries.GetAccountStats;
using SFA.DAS.EAS.Application.Queries.GetAccountTasks;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Application.Queries.GetTeamUser;
using SFA.DAS.EAS.Application.Queries.GetUserAccountRole;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerTeamOrchestratorTests
{
    public class WhenGettingAccount
    {
        private const string HashedAccountId = "ABC123";
        private const long AccountId = 123;
        private const string UserId = "USER1";

        private Mock<IMediator> _mediator;
        private EmployerTeamOrchestrator _orchestrator;
        private AccountStats _accountStats;
        private Mock<ICurrentDateTime> _currentDateTime;
 		private List<AccountTask> _tasks;
        private AccountTask _testTask;
        [SetUp]
        public void Arrange()
        {
            _accountStats = new AccountStats
            {
                AccountId = 10,
                OrganisationCount = 3,
                PayeSchemeCount = 4,
                TeamMemberCount = 8
            };

            _testTask = new AccountTask
            {
                Type = "Test",
                ItemsDueCount = 2
            };

            _tasks = new List<AccountTask>
            {
                _testTask
            };

            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.Is<GetEmployerAccountHashedQuery>(q => q.HashedAccountId == HashedAccountId)))
                .ReturnsAsync(new GetEmployerAccountResponse
                {
                    Account = new Domain.Data.Entities.Account.Account
                    {
                        HashedId = HashedAccountId,
                        Id = AccountId,
                        Name = "Account 1"
                    }
                });

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountTasksQuery>()))
                .ReturnsAsync(new GetAccountTasksResponse
                {
                    Tasks = _tasks
                });

            _mediator.Setup(m => m.SendAsync(It.Is<GetUserAccountRoleQuery>(q => q.ExternalUserId == UserId)))
                     .ReturnsAsync(new GetUserAccountRoleResponse
                     {
                         UserRole = Domain.Models.UserProfile.Role.Owner
                     });

            _mediator.Setup(m => m.SendAsync(It.Is<GetAccountEmployerAgreementsRequest>(q => q.HashedAccountId == HashedAccountId)))
                     .ReturnsAsync(new GetAccountEmployerAgreementsResponse
                     {
                         EmployerAgreements = new List<Domain.Models.EmployerAgreement.EmployerAgreementView>
                         {
                             new Domain.Models.EmployerAgreement.EmployerAgreementView
                             {
                                 Status = Domain.Models.EmployerAgreement.EmployerAgreementStatus.Pending
                             }
                         }
                     });

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetTeamMemberQuery>()))
                     .ReturnsAsync(new GetTeamMemberResponse{User = new MembershipView{FirstName = "Bob"}});

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountStatsQuery>()))
                     .ReturnsAsync(new GetAccountStatsResponse {Stats = _accountStats});
            
            _currentDateTime = new Mock<ICurrentDateTime>();

            _orchestrator = new EmployerTeamOrchestrator(_mediator.Object, _currentDateTime.Object);
        }
        
        [Test]
        public async Task ThenShouldGetAccountStats()
        {
            // Act
            var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

            //Assert
            Assert.IsNotNull(actual.Data);
            Assert.AreEqual(_accountStats.OrganisationCount, actual.Data.OrgainsationCount);
            Assert.AreEqual(_accountStats.PayeSchemeCount, actual.Data.PayeSchemeCount);
            Assert.AreEqual(_accountStats.TeamMemberCount, actual.Data.TeamMemberCount);
        }

        [Test]
        public async Task ThenShouldReturnTasks()
        {
            // Act
            var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

            //Assert
            Assert.IsNotNull(actual.Data);
            Assert.Contains(_testTask, actual.Data.Tasks.ToArray());
        }

        [Test]
        public async Task ThenIShouldNotReturnTasksWithZeroItems()
        {
            //Arrange
            _testTask.ItemsDueCount = 0;

            // Act
            var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

            //Assert
            Assert.IsNotNull(actual.Data);
            Assert.IsEmpty(actual.Data.Tasks);
        }

        [Test]
        public async Task ThenShouldReturnNoTasksIfANullIsReturnedFromTaskQuery()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountTasksQuery>()))
                .ReturnsAsync(null);

            // Act
            var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

            //Assert
            Assert.IsNotNull(actual.Data);
            Assert.IsEmpty(actual.Data.Tasks);
        }

        [Test]
        public async Task ThenShouldReturnAccountsTasks()
        {
            //Act
            var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

            //Assert
            Assert.AreEqual(_tasks, actual.Data.Tasks);
            _mediator.Verify(x => x.SendAsync(It.Is<GetAccountTasksQuery>(r => r.AccountId.Equals(AccountId))),Times.Once);
        }

        [Test]
        public async Task ThenShouldSetAccountHashId()
        {
            //Act
            var actual = await _orchestrator.GetAccount(HashedAccountId, UserId);

            //Assert
            Assert.AreEqual(HashedAccountId, actual.Data.HashedAccountId);
        }

        [TestCase("2017-10-19", true, Description = "Banner visible")]
        [TestCase("2017-10-19 11:59:59", true, Description = "Banner visible until midnight")]
        [TestCase("2017-10-20 00:00:00", false, Description = "Banner hidden after midnight")]
        public async Task ThenDisplayOfAcademicYearBannerIsDetermined(DateTime now, bool expectShowBanner)
        {
            //Arrange
            _currentDateTime.Setup(x => x.Now).Returns(now);

            //Act
            var model = await _orchestrator.GetAccount(HashedAccountId, UserId);

            //Assert
            Assert.AreEqual(expectShowBanner, model.Data.ShowAcademicYearBanner);
        }
    }
}
