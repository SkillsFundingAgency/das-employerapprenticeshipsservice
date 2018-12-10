using System;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.Dtos;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.ModelBuilders
{
    public class UserModelBuilder
    {
        public UserInput CreateUserInput()
        {
            var userRef = Guid.NewGuid().ToString();

            return new UserInput
            {
                UserRef = userRef,
                Email = userRef.Substring(0, 6) + ".madeupdomain.co.uk"
            };
        }

        public UserInput CreateUserInput(string userRef)
        {
            return new UserInput
            {
                UserRef = userRef,
                Email = userRef.Substring(0, 6) + ".madeupdomain.co.uk"
            };
        }
    }
}
