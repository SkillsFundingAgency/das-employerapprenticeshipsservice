using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper.Dtos;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper.Adapters
{
    public class UserInputToUserAdapter : User
    {
        public UserInputToUserAdapter(UserInput input)
        {
            Email = input.Email;
            FirstName = input.FirstName;
            LastName = input.LastName;
            ExternalId = input.ExternalId;
        }
    }
}