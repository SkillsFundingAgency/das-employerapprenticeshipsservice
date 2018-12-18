using SFA.DAS.EmployerFinance.Api.IntegrationTests.TestUtils.DataAccess.Dtos;
using SFA.DAS.EmployerFinance.Models.UserProfile;

namespace SFA.DAS.EmployerFinance.Api.IntegrationTests.TestUtils.DataAccess.Adapters
{
    public sealed class UserInputToUserAdapter : User
    {
        public UserInputToUserAdapter(UserInput input)
        {
            Email = input.Email;
            FirstName = input.FirstName;
            LastName = input.LastName;
            Ref = input.Ref;
        }
    }
}