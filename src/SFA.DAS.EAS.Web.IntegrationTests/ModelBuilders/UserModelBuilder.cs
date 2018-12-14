using System;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper.Dtos;

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
                Email = userRef.Substring(0, Math.Min(6, userRef.Length)) + ".madeupdomain.co.uk"
            };
        }
    }
}
