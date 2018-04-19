using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.TestCommon.Builders
{
    public class UserBuilder
    {
        private readonly User _user = new User();

        public User Build()
        {
            return _user;
        }
    }
}