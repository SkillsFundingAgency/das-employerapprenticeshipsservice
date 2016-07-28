using System.Linq;
using MediatR;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUserAccounts;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.DepedencyResolution;
using SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.DbCleanup;
using SFA.DAS.EmployerApprenticeshipsService.Web.DependencyResolution;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.Steps.InviteMember
{
    [Binding, Explicit]
    public class InviteMemberSteps
    {
        private IContainer _container;
        private string ExternalUserId;
        private int _accountId;

        [BeforeScenario]
        public void Arrange()
        {
            _container = new Container(c =>
            {
                c.Policies.Add<ConfigurationPolicy<EmployerApprenticeshipsServiceConfiguration>>();
                c.Policies.Add<LoggingPolicy>();
                c.Policies.Add(new MessagePolicy("SFA.DAS.EmployerApprenticeshipsService"));
                c.AddRegistry<DependencyResolution.DefaultRegistry>();
            });

            var cleanDownDb = _container.GetInstance<ICleanUpDatabase>();
            cleanDownDb.Execute().Wait();
        }

        [AfterScenario]
        public void TearDown()
        {
            _container.Dispose();
        }

        public StructureMapDependencyScope StructureMapDependencyScope { get; set; }


        [Given(@"I am an account owner")]
        public void GivenIAmAnAccountOwner()
        {
            var user = GetExistingUserAccount();
            ExternalUserId = user.UserId;

            CreateDasAccount(user);
        }

        [When(@"I invite a team member with email address ""(.*)"" and name ""(.*)""")]
        public void WhenIInviteATeamMemberWithEmailAddressAndName(string email, string name)
        {
            SetAccountIdForUser();

            CreateInvitationForGivenEmailAndName(email, name);
        }

        [Then(@"A user invite is ""(.*)""")]
        public void ThenAUserInviteIsWithPendingStatus(string createdStatus)
        {
            
            var orcehstrator = _container.GetInstance<EmployerTeamOrchestrator>();
            var teamMembers = orcehstrator.GetTeamMembers(_accountId, ExternalUserId).Result;

            if (createdStatus.ToLower() == "created")
            {
                Assert.AreEqual(2,teamMembers.TeamMembers.Count);
            }
            else
            {
                Assert.AreEqual(1, teamMembers.TeamMembers.Count);
            }
        }

        private void CreateInvitationForGivenEmailAndName(string email, string name)
        {
            var orcehstrator = _container.GetInstance<InvitationOrchestrator>();
            orcehstrator.CreateInvitation(new InviteTeamMemberViewModel
            {
                AccountId = _accountId,
                Email = email,
                Name = name,
                Role = Role.Transactor
            }, ExternalUserId).Wait();
        }

        private void SetAccountIdForUser()
        {
            var mediator = _container.GetInstance<IMediator>();
            var getUserAccountsQueryResponse = mediator.SendAsync(new GetUserAccountsQuery() {UserId = ExternalUserId}).Result;

            _accountId = getUserAccountsQueryResponse.Accounts.AccountList.FirstOrDefault().Id;
        }

        private SignInUserModel GetExistingUserAccount()
        {
            var homeOrchestrator = _container.GetInstance<HomeOrchestrator>();
            var user = homeOrchestrator.GetUsers().Result.AvailableUsers.FirstOrDefault();
            return user;
        }

        private void CreateDasAccount(SignInUserModel user)
        {
            var accountOrchestrator = _container.GetInstance<EmployerAccountOrchestrator>();

            accountOrchestrator.CreateAccount(new CreateAccountModel
            {
                CompanyName = "TestCompany",
                CompanyNumber = "123456",
                EmployerRef = "123/ABC123",
                UserId = user.UserId
            }).Wait();
        }
    }
}
