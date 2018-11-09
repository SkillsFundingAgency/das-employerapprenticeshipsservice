using MediatR;
using Moq;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Application.Commands.UpsertRegisteredUser;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.TestCommon.DependencyResolution;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Messaging.Interfaces;
using StructureMap;
using System;
using System.Linq;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.TestCommon.ScenarioCommonSteps
{
    public class UserSteps
    {
        private IContainer _container;
        private Mock<IMessagePublisher> _messagePublisher;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
        private Mock<IEventsApi> _eventsApi;
        private Mock<IEmployerCommitmentApi> _commitmentsApi;

        public UserSteps()
        {
            _messagePublisher = new Mock<IMessagePublisher>();
            _owinWrapper = new Mock<IAuthenticationService>();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            _eventsApi = new Mock<IEventsApi>();
            _commitmentsApi = new Mock<IEmployerCommitmentApi>();

            _container = IoC.CreateContainer(_messagePublisher, _owinWrapper, _cookieService, _eventsApi, _commitmentsApi);
        }

        public void UpsertUser(UserViewModel userView)
        {
            var mediator = _container.GetInstance<IMediator>();

            mediator.SendAsync(new UpsertRegisteredUserCommand
            {
                UserRef = userView.UserRef,
                FirstName = userView.FirstName,
                LastName = userView.LastName,
                EmailAddress = userView.Email
            }).Wait();
        }


        public UserViewModel GetExistingUserAccount()
        {
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns(ScenarioContext.Current["AccountOwnerUserRef"].ToString());
            var orchestrator = _container.GetInstance<HomeOrchestrator>();
            var user = orchestrator.GetUsers().Result.AvailableUsers.FirstOrDefault(c => c.UserRef.Equals(ScenarioContext.Current["AccountOwnerUserRef"].ToString(), StringComparison.CurrentCultureIgnoreCase));
            return user;
        }

        public UserViewModel GetExistingUserAccount(string userRef)
        {
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns(userRef);
            var orchestrator = _container.GetInstance<HomeOrchestrator>();
            var user = orchestrator.GetUsers().Result.AvailableUsers.FirstOrDefault(c => c.UserRef.Equals(userRef, StringComparison.CurrentCultureIgnoreCase));
            return user;
        }

        public long CreateUserWithRole(User user, Role role, long accountId)
        {
            var userRepository = _container.GetInstance<IUserRepository>();
            var membershipRepository = _container.GetInstance<IMembershipRepository>();
            var userRecord = userRepository.GetUserByRef(user.UserRef).Result;

            membershipRepository.Create(userRecord.Id, accountId, (short)role).Wait();

            return userRecord.Id;
        }
    }
}
