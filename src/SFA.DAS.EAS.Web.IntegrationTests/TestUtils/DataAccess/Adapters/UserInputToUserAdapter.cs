using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.Dtos;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.Adapters
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