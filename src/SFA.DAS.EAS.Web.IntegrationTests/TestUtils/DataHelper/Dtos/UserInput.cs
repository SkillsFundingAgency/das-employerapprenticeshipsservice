using System;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper.Dtos
{
    public class UserInput
    {
        public Guid ExternalId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}