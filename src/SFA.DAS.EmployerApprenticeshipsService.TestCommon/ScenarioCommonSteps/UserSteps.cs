using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using SFA.DAS.EAS.Application.Commands.UpsertRegisteredUser;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.TestCommon.ScenarioCommonSteps
{
    public class UserSteps
    {
        public static void UpsertUser(IMediator mediator, SignInUserModel user)
        {
            mediator.SendAsync(new UpsertRegisteredUserCommand
            {
                UserRef = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.Email
            }).Wait();
        }

        public static SignInUserModel GetExistingUserAccount(string accountOwnerUserId, Mock<IOwinWrapper> owinwrapper, HomeOrchestrator orchestrator)
        {
            owinwrapper.Setup(x => x.GetClaimValue("sub")).Returns(accountOwnerUserId);
            
            var user = orchestrator.GetUsers().Result.AvailableUsers.FirstOrDefault(c => c.UserId.Equals(accountOwnerUserId, StringComparison.CurrentCultureIgnoreCase));
            return user;
        }

        public static long CreateUserWithRole(User user, Role role, long accountId, IUserRepository userRepository, IMembershipRepository membershipRepository)
        {
            var userRecord = userRepository.GetByUserRef(user.UserRef).Result;

            membershipRepository.Create(userRecord.Id, accountId, (short)role).Wait();

            return userRecord.Id;
        }
    }
}
