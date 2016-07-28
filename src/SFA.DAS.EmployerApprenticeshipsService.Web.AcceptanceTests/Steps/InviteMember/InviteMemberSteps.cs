using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Web.DependencyResolution;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.Steps.InviteMember
{
    [Binding]
    public class InviteMemberSteps
    {
        private IContainer _container;
        private string ExternalUserId;

        [BeforeScenario()]
        public void Arrange()
        {
            _container = IoC.Initialize();
            
            StructureMapDependencyScope = new StructureMapDependencyScope(_container);
            
        }

        [AfterScenario()]
        public void TearDown()
        {
            StructureMapDependencyScope.Dispose();
            _container.Dispose();
        }

        public StructureMapDependencyScope StructureMapDependencyScope { get; set; }


        [Given(@"I am an account owner")]
        public void GivenIAmAnAccountOwner()
        {
            try
            {
                //var account = _container.GetInstance<IAccountRepository>();

                var homeOrchestrator = _container.GetInstance<HomeOrchestrator>();
                var accountOrchestrator = _container.GetInstance<EmployerAccountOrchestrator>();

                //Get user
                var users = homeOrchestrator.GetUsers().Result;
                var user = users.AvailableUsers.FirstOrDefault();

                //Create account
                accountOrchestrator.CreateAccount(new CreateAccountModel
                {
                    CompanyName = "TestCompany",
                    CompanyNumber = "123456",
                    EmployerRef = "123/ABC123",
                    UserId = user.UserId
                }).Wait();

                ExternalUserId = user.UserId;
            }
            catch (Exception ex)
            {
                
                throw;
            }
            
        }

        [When(@"I invite a team member with email address ""(.*)"" and name ""(.*)""")]
        public void WhenIInviteATeamMemberWithEmailAddressAndName(string email, string name)
        {
            var orcehstrator = _container.GetInstance<InvitationOrchestrator>();
            orcehstrator.CreateInvitation(new InviteTeamMemberViewModel
            {
                AccountId = 1, Email = email, Name = name, Role = Role.Transactor
            }, ExternalUserId).Wait();
        }

        [Then(@"A user invite is ""(.*)"" with pending status")]
        public void ThenAUserInviteIsWithPendingStatus(string createdStatus)
        {
            //Check invites table
        }

    }
}
