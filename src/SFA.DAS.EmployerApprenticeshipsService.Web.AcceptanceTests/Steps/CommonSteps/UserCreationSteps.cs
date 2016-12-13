using System;
using System.Linq;
using MediatR;
using Moq;
using SFA.DAS.EAS.Application.Commands.UpsertRegisteredUser;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.TestCommon.DependencyResolution;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.Messaging;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.Web.AcceptanceTests.Steps.CommonSteps
{
    public class UserCreationSteps
    {
        private IContainer _container;
        private Mock<IMessagePublisher> _messagePublisher;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<ICookieService> _cookieService;

        public UserCreationSteps()
        {
            _messagePublisher = new Mock<IMessagePublisher>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _cookieService = new Mock<ICookieService>();

            _container = IoC.CreateContainer(_messagePublisher, _owinWrapper, _cookieService);
        }

        public void UpsertUser(SignInUserModel user)
        {
            var mediator = _container.GetInstance<IMediator>();

            mediator.SendAsync(new UpsertRegisteredUserCommand
            {
                UserRef = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.Email
            }).Wait();
        }

        public SignInUserModel GetExistingUserAccount()
        {
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns(ScenarioContext.Current["AccountOwnerUserId"].ToString());
            var orchestrator = _container.GetInstance<HomeOrchestrator>();
            var user = orchestrator.GetUsers().Result.AvailableUsers.FirstOrDefault(c=>c.UserId.Equals(ScenarioContext.Current["AccountOwnerUserId"].ToString(), StringComparison.CurrentCultureIgnoreCase));
            return user;
        }

        public long CreateUserWithRole(User user,Role role, long accountId)
        {
            var userRepository = _container.GetInstance<IUserRepository>();
            var membershipRepository = _container.GetInstance<IMembershipRepository>();
            var userRecord = userRepository.GetByUserRef(user.UserRef).Result;

            membershipRepository.Create(userRecord.Id, accountId, (short) role).Wait();

            return userRecord.Id;
        }
    }
}
