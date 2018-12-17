using System;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataHelper.Dtos;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.ModelBuilders
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
