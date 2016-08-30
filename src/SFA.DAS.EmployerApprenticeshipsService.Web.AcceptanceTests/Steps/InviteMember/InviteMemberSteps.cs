using System;
using System.Linq;
using System.Net;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUserAccounts;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.TestCommon.DbCleanup;
using SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.DependencyResolution;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.Messaging;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.Steps.InviteMember
{
    [Binding, Explicit]
    public class InviteMemberSteps
    {
        private static IContainer _container;
        private long _accountId;
        private static Mock<IMessagePublisher> _messagePublisher;
        private static Mock<IOwinWrapper> _owinWrapper;


        [BeforeFeature]
        public static void Arrange()
        {
            _messagePublisher = new Mock<IMessagePublisher>();
            _owinWrapper = new Mock<IOwinWrapper>();

            _container = IoC.CreateContainer(_messagePublisher, _owinWrapper);
            
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
            var teamMembers = orcehstrator.GetTeamMembers(_accountId, accountOwnerId).Result;

            var invitedTeamMember = ScenarioContext.Current["InvitedEmailAddress"].ToString();

            if (createdStatus.ToLower() == "created")
            {
                Assert.IsTrue(teamMembers.Data.TeamMembers.Any(c=>c.Email.Equals(invitedTeamMember,StringComparison.CurrentCultureIgnoreCase)));
                //Check to make sure an email has been sent
                _messagePublisher.Verify(x=>x.PublishAsync(It.IsAny<SendNotificationQueueMessage>()), Times.AtLeastOnce);
            }
            else
            {
                Assert.IsFalse(teamMembers.Data.TeamMembers.Any(c => c.Email.Equals(invitedTeamMember, StringComparison.CurrentCultureIgnoreCase)));
                //Check to make sure an email has not been sent
                //_messagePublisher.Verify(x => x.PublishAsync(It.IsAny<SendNotificationQueueMessage>()), Times.Never);
            }
        }

        [Then(@"I ""(.*)"" view team members")]
        public void ThenIViewTeamMembers(string canView)
        {
            var userId = ScenarioContext.Current["ExternalUserId"].ToString();
            var accountId = (long)ScenarioContext.Current["AccountId"];
            var orcehstrator = _container.GetInstance<EmployerTeamOrchestrator>();
            var teamMembers = orcehstrator.GetTeamMembers(accountId, userId).Result;
            
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
                AccountId = _accountId,
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

            _accountId = getUserAccountsQueryResponse.Accounts.AccountList.FirstOrDefault().Id;
        }
        
    }
}
