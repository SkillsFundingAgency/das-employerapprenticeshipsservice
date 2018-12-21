using SFA.DAS.EmployerAccounts.Models.UserProfile;

namespace SFA.DAS.EmployerAccounts.TestCommon.Builders
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