using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper.Dtos;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper.Adapters
{
    public class UserInputToUserAdapter : User
    {
        public UserInputToUserAdapter(UserInput input)
        {
            this.Email = input.Email;
            this.FirstName = input.FirstName;
            this.LastName = input.LastName;
            this.UserRef = input.UserRef;
        }
    }
}