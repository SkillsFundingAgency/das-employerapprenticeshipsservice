using System.Linq;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.UpsertRegisteredUser;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.Steps.CommonSteps
{
    public static class UserCreationSteps
    {
        public static void UpsertUser(SignInUserModel user, IMediator mediator)
        {
            mediator.SendAsync(new UpsertRegisteredUserCommand
            {
                UserRef = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.Email
            }).Wait();
        }

        public static SignInUserModel GetExistingUserAccount(HomeOrchestrator orchestrator)
        {
            var user = orchestrator.GetUsers().Result.AvailableUsers.FirstOrDefault();
            return user;
        }

        public static long CreateUserWithRole(User user,Role role, long accountId, IUserRepository userRepository, IMembershipRepository membershipRepository)
        {
            var userRecord = userRepository.GetById(user.UserRef).Result;

            membershipRepository.Create(userRecord.Id, accountId, (short) role).Wait();

            return userRecord.Id;
        }
    }
}
