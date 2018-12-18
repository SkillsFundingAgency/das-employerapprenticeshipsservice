using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataHelper.Dtos;
using SFA.DAS.EmployerAccounts.Models.UserProfile;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataHelper.Adapters
{
    public class UserInputToUserAdapter : User
    {
        public UserInputToUserAdapter(UserInput input)
        {
            Email = input.Email;
            FirstName = input.FirstName;
            LastName = input.LastName;
            UserRef = input.UserRef;
        }
    }
}