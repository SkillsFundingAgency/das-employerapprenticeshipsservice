using System;
using System.Linq;
using System.Net;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetUserAccounts;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.TestCommon.DependencyResolution;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.Messaging;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.Web.AcceptanceTests.Steps.InviteMember
{
    [Binding, Explicit]
    public class InviteMemberSteps
    {
        private static IContainer _container;
        private static Mock<IMessagePublisher> _messagePublisher;
        private static Mock<IOwinWrapper> _owinWrapper;
        private string _hashedAccountId;
        private static Mock<ICookieService> _cookieService;


        [BeforeFeature]
        public static void Arrange()
        {
            _messagePublisher = new Mock<IMessagePublisher>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _cookieService = new Mock<ICookieService>();

            _container = IoC.CreateContainer(_messagePublisher, _owinWrapper, _cookieService);
            
        }

        [AfterFeature]
        public static void TearDown()
        {
            _container.Dispose();
        }
        
        [When(@"I invite a team member with email address ""(.*)"" and name ""(.*)""")]
        public void WhenIInviteATeamMemberWithEmailAddressAndName(string email, string name)
        {
            SetAccountIdForUser();

            ScenarioContext.Current["InvitedEmailAddress"] = email;
            CreateInvitationForGivenEmailAndName(email, name);
        }

        [Then(@"A user invite is ""(.*)""")]
        public void ThenAUserInviteIsWithPendingStatus(string createdStatus)
        {
            var accountOwnerId = ScenarioContext.Current["AccountOwnerUserId"].ToString();
            var orcehstrator = _container.GetInstance<EmployerTeamOrchestrator>();
            var teamMembers = orcehstrator.GetTeamMembers(_hashedAccountId, accountOwnerId).Result;

            var invitedTeamMember = ScenarioContext.Current["InvitedEmailAddress"].ToString();

            if (createdStatus.ToLower() == "created")
            {
                Assert.IsTrue(teamMembers.Data.TeamMembers.Any(c=>c.Email.Equals(invitedTeamMember,StringComparison.CurrentCultureIgnoreCase)));
            }
            else
            {
                Assert.IsFalse(teamMembers.Data.TeamMembers.Any(c => c.Email.Equals(invitedTeamMember, StringComparison.CurrentCultureIgnoreCase)));
            }
        }

        [Then(@"I ""(.*)"" view team members")]
        public void ThenIViewTeamMembers(string canView)
        {
            var userId = ScenarioContext.Current["ExternalUserId"].ToString();
            var hashedId = (string)ScenarioContext.Current["HashedAccountId"];
            var orcehstrator = _container.GetInstance<EmployerTeamOrchestrator>();
            var teamMembers = orcehstrator.GetTeamMembers(hashedId, userId).Result;
            
            if (canView.ToLower().Equals("can"))
            {
                Assert.AreEqual(HttpStatusCode.OK, teamMembers.Status);
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, teamMembers.Status);
            }
        }


        private void CreateInvitationForGivenEmailAndName(string email, string name)
        {
            var accountOwnerId = ScenarioContext.Current["AccountOwnerUserId"].ToString();
            var orchestrator = _container.GetInstance<InvitationOrchestrator>();
            orchestrator.CreateInvitation(new InviteTeamMemberViewModel
            {
                HashedAccountId = _hashedAccountId,
                Email = email,
                Name = name,
                Role = Role.Transactor
            }, accountOwnerId).Wait();
        }

        private void SetAccountIdForUser()
        {
            var accountOwnerId = ScenarioContext.Current["AccountOwnerUserId"].ToString();
            var mediator = _container.GetInstance<IMediator>();
            var getUserAccountsQueryResponse = mediator.SendAsync(new GetUserAccountsQuery {UserId = accountOwnerId }).Result;

            var account = getUserAccountsQueryResponse.Accounts.AccountList.FirstOrDefault();
            _hashedAccountId = account?.HashedId;
        }
        
    }
}
